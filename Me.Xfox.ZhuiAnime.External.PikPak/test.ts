import { Importer } from "./importer.ts";

const importer = new Importer();
await importer.init();

for (const anime of importer.config.anime) {
  const za_item = await importer.import_bgm_subject(anime.bangumi);
  const items = await importer.fetch_rss_source(anime.source);
  for (const item of items) {
    const [task_id, file_id, file_name] =
      (await importer.fetch_rss_item(item)) ?? [];
    if (!task_id || !file_id) continue;
    const target_folder_id = await importer.pikpak.resolve_folder_id(
      anime.target
    );
    await importer.pikpak.move(file_id, target_folder_id);
    const episode =
      item.title?.match(new RegExp(anime.regex))?.groups?.ep ?? "";
    if (!episode) continue;
    await importer.add_link(`${anime.target}/${file_name}`, za_item, episode);
  }
}

await importer.db.write();
