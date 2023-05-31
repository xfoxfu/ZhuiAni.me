export interface Config {
  username: string;
  password: string;
  anime: {
    source: string;
    target: string;
    regex: string;
    bangumi: number;
  }[];
}

export interface Data {
  refresh_token: string;
  access_token: string;
  // URL -> [TaskId, FileId, FileName]
  downloaded_files: Record<string, [string, string, string]>;
}
