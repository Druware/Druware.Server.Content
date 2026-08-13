## 2026-08-13

* Added seven download link properties to `Product`, stored as first-class
  nullable columns alongside the existing `download_url`,
  `documentation_url`, and `icon_url`: `app_store_apple`, `app_store_ms`,
  `app_store_google`, `app_store_amazon`, `direct_osx`, `direct_win_arm`,
  and `direct_win_x64`. All are `string?` with a maximum length of 278,
  matching the `DownloadUrl` idiom. `DownloadUrl` itself is unchanged.
  Migrations added for Microsoft, PostgreSql, and Sqlite. These are
  additive nullable columns, so this is not a breaking change — existing
  rows and any code that ignores the new properties continue to work.

## 2026-07-28

* Added a `ProductMeta` entity for attaching arbitrary property/value
  pairs to a `Product`, mapped to `content.product_meta` with a unique
  index on `(product_id, property)` and an FK back to `content.product`.
  Exposed via a `ProductMeta` navigation collection and a `[NotMapped]
  Meta` dictionary projection on `Product`, mirroring the existing
  `ProductTags`/`Tags` pattern. Migrations added for Microsoft,
  PostgreSql, and Sqlite.
* Fixed `ContentContextPostgreSql`, which never applied
  `ProductTagConfiguration`. As a result PostgreSQL had been mapping
  `ProductTag` entirely by convention — table `ProductTags` in the
  default schema with PascalCase columns — while SQL Server and SQLite
  used `content.product_tag` with snake_case. **This is a breaking
  schema change for existing PostgreSQL deployments**: the
  `ProductTagMapping` migration renames `ProductTags` to
  `content.product_tag` and renames its `Id`, `TagId`, and `ProductId`
  columns to `id`, `tag_id`, and `product_id`, rebuilding the primary
  key and both foreign keys. Anything outside EF Core that references
  the old table or column names will break — review this migration
  before applying it to a PostgreSQL database.

## 2026-07-24

* Floated `Microsoft.AspNetCore.Identity` from the exact 2.3.1 pin to 2.3.*.
  Druware.Server 1.1.16 requires 2.3.11, and the exact pin made that a package
  downgrade, which fails the build outright rather than warning.

## 2026-01-02

* Removed .nuspec
* Updated build scripts to push to both nuget.org and satori.
* Updated Nuget Packages
