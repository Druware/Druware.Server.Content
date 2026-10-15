## 2026-10-15

* Version 1.3.1, a minor move from the 1.2 line.
* Added a `SiteSetting` entity for site-wide key/value settings (links,
  list names, pointers to documents), mapped to `content.site_setting`
  with a `bigint` identity key `setting_id`, a required `setting_key` of
  at most 128 characters with a unique index, an unbounded `value`, an
  optional `description` of at most 512 characters and a required
  `updated` stamp that callers set on every write. Adding a setting is a
  new row, not a schema change. The library seeds no settings and
  defines no endpoint; the entity carries scalars only, so it serialises
  without `$id`/`$ref` wrappers. Values are meant to be publicly
  readable, so never store secrets in it. `SiteSetting.ByKeyOrId`
  looks a row up by id or exact key, and `SiteSetting.IsValidKey` and
  `SiteSetting.Validate` hold the key and length rules (a key starts
  with a letter and is letters, digits and dots with no empty segments)
  so controllers stay thin. The mapping is one provider-neutral
  `SiteSettingConfiguration`; PostgreSQL alone overrides `updated` to
  `timestamp without time zone`, matching the other content timestamps.
  Migrations added for Microsoft, PostgreSql, and Sqlite. This is
  additive, so not a breaking change.
* Added `SiteContentSecurityRole` (Author/Editor), one pair for the
  site-content family of entities, and seeded the two roles ("Site
  Content Author", "Site Content Editor") from
  `ContentContext.ConfigureSecurityRoles`.
* Added the `UnitTests` xUnit project to the solution and re-enabled
  the Test step in the publish workflow, so a failing test now blocks
  publishing. The tests run against an in-memory Sqlite database: every
  Sqlite migration applies to an empty database, `SiteSetting` round
  trips, duplicate keys are rejected, `ByKeyOrId` and the key rules
  behave, and `ConfigureSecurityRoles` is idempotent.

## 2026-09-12

* Added four link properties to `Product`, stored as nullable columns
  alongside the existing direct download links: `direct_gnome`,
  `direct_kde`, `direct_haiku`, and `repo_github`. All are `string?` with a
  maximum length of 278, matching the `DownloadUrl` idiom. Migrations added
  for Microsoft, PostgreSql, and Sqlite. These are additive nullable
  columns, so this is not a breaking change.

## 2026-08-31

* Added a `Collection` entity for grouping `Product` entities, mapped to
  `content.collection` with a unique index on `name` and DB-side
  `created`/`updated` defaults per provider, matching the `Product`
  idiom. Membership is carried by an explicit join entity,
  `CollectionProduct`, mapped to `content.collection_product` with a
  unique index on `(collection_id, product_id)` so a product cannot be
  added to the same collection twice, and foreign keys back to both
  `content.collection` and `content.product`. Grouping is unordered and
  tag-like. Exposed via a `CollectionProducts` navigation collection on
  both entities, a `[NotMapped] Products` string array on `Collection`,
  and a read-only `[NotMapped] Collections` string array on `Product` --
  membership is managed from the `Collection` side. Unlike
  `Tag.ByNameOrId`, `Collection.ByNameOrId` does not auto-create a
  missing row; collections are curated entities with their own CRUD.
  Added `CollectionSecurityRole` (Author/Editor) and seeded the two
  roles from `ContentContext.ConfigureSecurityRoles`. Migrations added
  for Microsoft, PostgreSql, and Sqlite. Everything here is additive, so
  this is not a breaking change.

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
