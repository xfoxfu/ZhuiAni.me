import ky from "https://esm.sh/ky@0.33.3";
import {
  NewFile,
  File,
  FileList,
  RequestNewFile,
  KindOfFolder,
  KindOfFile,
} from "./pikpak_types.ts";

export class PikPak {
  public client: ReturnType<(typeof ky)["create"]>;
  public refresh_token: string;
  public access_token: string;

  constructor(readonly username: string, readonly password: string) {
    this.client = ky.create({
      headers: {
        "User-Agent": "",
      },
      hooks: {
        beforeError: [
          async (err) => {
            const body = await err.response.json();
            console.log(err.message, body.error_description);
            return err;
          },
        ],
      },
    });
    this.refresh_token = "";
    this.access_token = "";
  }

  login = async () => {
    const res = await this.client.post(
      "https://user.mypikpak.com/v1/auth/signin",
      {
        json: {
          captcha_token: "",
          client_id: "YNxT9w7GMdWvEOKa",
          client_secret: "dbw2OtmVEeuUvIptb1Coyg",
          username: this.username,
          password: this.password,
        },
      }
    );
    // deno-lint-ignore no-explicit-any
    const body = (await res.json()) as any;
    this.use_token(body.refresh_token, body.access_token);
  };

  use_token = (refresh_token: string, access_token: string) => {
    this.refresh_token = refresh_token;
    this.access_token = access_token;
    this.client = this.client.extend({
      headers: {
        Authorization: `Bearer ${this.access_token}`,
      },
    });
  };

  refresh_login = async () => {
    const res = await this.client.post(
      "https://user.mypikpak.com/v1/auth/token",
      {
        json: {
          client_id: "YNxT9w7GMdWvEOKa",
          client_secret: "dbw2OtmVEeuUvIptb1Coyg",
          grant_type: "refresh_token",
          refresh_token: this.refresh_token,
        },
      }
    );
    // deno-lint-ignore no-explicit-any
    const body = (await res.json()) as any;
    this.use_token(body.refresh_token, body.access_token);
  };

  download = async (url: string): Promise<NewFile> => {
    const res = await this.client.post(
      "https://api-drive.mypikpak.com/drive/v1/files",
      {
        json: {
          kind: KindOfFile,
          name: "",
          upload_type: "UPLOAD_TYPE_URL",
          url: { url },
          folder_type: "DOWNLOAD",
        },
      }
    );
    return await res.json();
  };

  create_folder = async (parent_id: string, name: string): Promise<NewFile> => {
    const res = await this.client.post(
      "https://api-drive.mypikpak.com/drive/v1/files",
      {
        json: {
          kind: KindOfFolder,
          name: name,
          parent_id: parent_id,
          folder_type: "",
        } satisfies RequestNewFile,
      }
    );
    return await res.json();
  };

  wait_task = async (task_id: string) => {
    while (true) {
      const res = await this.client.get(
        "https://api-drive.mypikpak.com/drive/v1/tasks/" + task_id
      );
      // deno-lint-ignore no-explicit-any
      const body = (await res.json()) as any;
      if (
        body.phase === "PHASE_TYPE_COMPLETE" ||
        body.phase === "PHASE_TYPE_ERROR"
      ) {
        return;
      }
      await new Promise((res) => setTimeout(res, 1000));
    }
  };

  list = async (folder_id?: string): Promise<FileList> => {
    const res = await this.client.get(
      "https://api-drive.mypikpak.com/drive/v1/files",
      { searchParams: folder_id && { parent_id: folder_id } }
    );
    return await res.json();
  };

  get_file = async (file_id: string): Promise<File> => {
    const res = await this.client.get(
      "https://api-drive.mypikpak.com/drive/v1/files/" + file_id
    );
    return await res.json();
  };

  move = async (file_id: string, target_id: string): Promise<void> => {
    await this.client.post(
      "https://api-drive.mypikpak.com/drive/v1/files:batchMove",
      {
        json: {
          ids: [file_id],
          to: { parent_id: target_id },
        },
      }
    );
  };

  resolve_folder_id = async (
    path: string[] | string,
    parent_id?: string
  ): Promise<string> => {
    if (typeof path === "string") {
      if (path.startsWith("/")) path = path.slice(1);
      path = path.split("/");
    }
    const [folder_name, ...rest] = path;
    const folder = await this.list(parent_id);
    const target = folder.files?.find(
      (f) => f?.name === folder_name && !f?.trashed
    );
    const folder_id =
      target?.id ??
      (await this.create_folder(parent_id ?? "", folder_name))?.file?.id;
    if (!folder_id) throw new Error("unexpected");
    if (rest.length > 0) {
      return await this.resolve_folder_id(rest, folder_id);
    }
    return folder_id;
  };
}
