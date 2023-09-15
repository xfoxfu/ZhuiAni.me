# [ZhuiAni.me](https://zhuiani.me)

[![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/xfoxfu/ZhuiAni.me/ci.yml?branch=main&style=flat-square)](https://github.com/xfoxfu/ZhuiAni.me/actions/workflows/ci.yml)
[![License](https://img.shields.io/github/license/xfoxfu/ZhuiAni.me?style=flat-square)](https://github.com/xfoxfu/ZhuiAni.me/blob/main/LICENSE)
[![Codecov](https://img.shields.io/codecov/c/github/xfoxfu/ZhuiAni.me?style=flat-square&logo=codecov&logoColor=white)](https://codecov.io/gh/xfoxfu/ZhuiAni.me)
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fxfoxfu%2FZhuiAni.me.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2Fxfoxfu%2FZhuiAni.me?ref=badge_shield)

A platform for scraping, subscribing, downloading and watching animations based on torrents.

## Components

- `src`: A Web App for listing & managing animation and torrent metadata.
- ~~A scraper for torrents.~~
- ~~A Web App for downloading & watching animation.~~

## How to deploy

The application requires a working PostgreSQL instance to be running.

### Migrations

To execute migrations, run the binary with `migrate` command.

```console
./Me.Xfox.ZhuiAnime migrate
```

If you want to run migrations on a different database, add `--ConnectionStrings:ZAContext=` and then append the database connection string.

```console
./Me.Xfox.ZhuiAnime --ConnectionStrings:ZAContext='Host=localhost;Port=5432;Username=postgres;Password=;Database=zhuianime;Include Error Detail=true'
```

### Configuration

The program will lookup for configurations in `appsettings.Local.toml`. Some pre-defined configurations are in `appsettings.toml`, but it is recommended to keep this file as-is, since it could be modified with updates.

<!-- prettier-ignore -->
| Path | Description | Default Value |
| --- | --- | --- |
| `ConnectionStrings:ZAContext` | [database connection string](https://learn.microsoft.com/en-us/ef/core/miscellaneous/connection-strings) | `Host=localhost;Database=zhuianime;Include Error Detail=true` |
| `Modules:TorrentDirectory:IntervalBetweenFetch` | interval between fetching the torrent source site for an update | 5 minutes |
| `Modules:TorrentDirectory:IntervalBetweenFetch` | interval between pages of a single fetch to torrent site | 10 seconds |
| `Modules:TorrentDirectory:FetchPageThreshold` | maximum pages to fetch for update | `20` |
| `Modules:TorrentDirectory:Sources:bangumi.moe` | whether to fetch <https://bangumi.moe> | `true` |
| `Modules:TorrentDirectory:Sources:acg.rip` | whether to fetch <https://acg.rip> | `true` |
| `Modules:PikPak:Username` | username of PikPak |  |
| `Modules:PikPak:Password` | password of PikPak |  |
| `Modules:PikPak:AccessAddressTemplate` | URL template used for accessing PikPak's files. `{0}` stands for PikPak path. | `{0}` |
| `Modules:PikPak:IntervalBetweenFetch` | unused | 1 hour |
| `Captcha:Enabled` | whether to use [Turnstile](https://www.cloudflare.com/products/turnstile/) to protect login and sign-up APIs | `true` |
| `Captcha:SiteKey` | site key of Turnstile |  |
| `Captcha:Secret` | secret key of Turnstile |  |
| `Authentication:Jwt:PrivateKey` | ECC private key used to sign JWT ([recommended generation tool](https://dinochiesa.github.io/jwt/)) |  |
| `Authentication:Jwt:PublicKey` | ECC public key used to sign JWT |  |
| `Authentication:Jwt:Issuer` | the issuer of tokens | `https://zhuiani.me` |
| `Authentication:Jwt:AudienceFirstParty` | the audience of tokens issued to first-party applications (the built-in frontend) | `https://zhuiani.me` |
| `Authentication:Jwt:FirstPartyExpires` | the expiration of access tokens issued to first-party applications | 10 minutes |
| `Authentication:Jwt:RefreshExpiresDays` | expiration of refresh tokens (in days) | 7 days |

## License

```
The MIT License (MIT)

Copyright © 2021 xfoxfu

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
```

[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fxfoxfu%2FZhuiAni.me.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2Fxfoxfu%2FZhuiAni.me?ref=badge_large)
