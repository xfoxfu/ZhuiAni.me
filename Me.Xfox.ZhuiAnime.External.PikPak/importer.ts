import { Low } from "https://esm.sh/lowdb@6.0.1";
import { Config, Data } from "./db.ts";
import { ItemDto, Api as ZAApi } from "./api.ts";
import { JSONFile } from "https://esm.sh/lowdb@6.0.1/node";
import { PikPak } from "./pikpak.ts";
import { parse as tomlParse } from "https://esm.sh/@ltd/j-toml@1.38.0";
import Parser, { Item as RssItem } from "https://esm.sh/rss-parser@3.13.0";

export class Importer {
  public db: Low<Data>;
  public za: ZAApi<unknown>;
  public pikpak: PikPak;
  public config: Config;
  public rss: Parser;
  public cache: { sources: Record<string, RssItem[]> };

  constructor() {
    this.db = new Low<Data>(new JSONFile("./db.json"), {
      refresh_token: "",
      access_token: "",
      downloaded_files: {},
    });
    this.za = new ZAApi({
      baseUrl: "http://localhost:5000",
    });
    this.pikpak = new PikPak("", "");
    this.config = null as unknown as Config;
    this.rss = new Parser();
    this.cache = { sources: {} };
  }

  init = async () => {
    await this.db.read();
    this.config = tomlParse(
      await Deno.readFile("./config.toml")
    ) as unknown as Config;
    this.pikpak = new PikPak(this.config.username, this.config.password);
    this.pikpak.use_token(
      this.db.data.refresh_token,
      this.db.data.access_token
    );
    if (this.db.data.access_token.length === 0) {
      await this.pikpak.login();
    } else {
      try {
        await this.pikpak.list();
      } catch {
        try {
          await this.pikpak.refresh_login();
        } catch {
          await this.pikpak.login();
        }
      }
    }
    this.db.data.refresh_token = this.pikpak.refresh_token;
    this.db.data.access_token = this.pikpak.access_token;
  };

  fetch_rss_source = async (source_url: string): Promise<RssItem[]> => {
    if (this.cache.sources[source_url]) return this.cache.sources[source_url];

    const feed = await this.rss.parseURL(source_url);
    const items = feed.items.filter((i) => !!i);
    this.cache.sources[source_url] = items;
    return items;
  };

  fetch_rss_item = async (
    item: RssItem
  ): Promise<[string, string, string] | null> => {
    if (!item.enclosure?.url) return null;
    const url = item.enclosure?.url;
    if (this.db.data.downloaded_files[url]) {
      return this.db.data.downloaded_files[url];
    }
    const newFile = await this.pikpak.download(item.enclosure?.url);
    if (!newFile.task?.id || !newFile.task.file_id || !newFile.task.file_name) {
      throw new Error("download failed");
    }
    await this.pikpak.wait_task(newFile.task.id);
    this.db.data.downloaded_files[url] = [
      newFile.task.id,
      newFile.task.file_id,
      newFile.task.file_name,
    ];
    return this.db.data.downloaded_files[url];
  };

  import_bgm_subject = async (subject_id: number): Promise<ItemDto> => {
    const za_item = await this.za.bangumi.postExternalBangumiImportSubject({
      id: Number(subject_id),
    });
    return za_item.data;
  };

  add_link = async (path: string, RssItem: ItemDto, episode_id: string) => {
    const file_path = path
      .split("/")
      .map((s) => encodeURIComponent(s ?? ""))
      .join("/");

    const episodes = await this.za.item.getItemItems(RssItem.id);
    const episode = episodes.data.find(
      (e) =>
        Number.parseFloat(e.annotations["https://bgm.tv/ep/:id/sort"]) ===
        Number.parseFloat(episode_id)
    );
    const address = `https://alist.xfox.me${file_path}`;
    if (
      (
        await this.za.itemLink.getItemLinks(episode?.id ?? RssItem.id)
      ).data.filter((l) => decodeURI(l.address) === decodeURI(address))
        .length === 0
    ) {
      await this.za.itemLink.postItemLinks(episode?.id ?? RssItem.id, {
        address: address,
        mime_type: "text/html;kind=video",
        annotations: {},
      });
    }
  };
}
