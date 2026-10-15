## 2026-10-15 - reviews

* Version 1.4.0, a minor move from the 1.3 line for a new entity; CI bumps the patch and publishes 1.4.1.
* Added a `Review` entity for gear reviews, mapped to `content.review` with a `bigint` identity key `review_id`: a required `title` (255) and `product_name` (200), an optional `permalink` (255) with a unique index `ix_review_permalink`, `category` (64), `summary` (1024), an unbounded `body`, `product_url` (2048), `is_affiliate`, two optional scores `eddie_score` and `dru_score`, two optional takes `eddie_take` and `dru_take` (280, with `Article`'s rules), `posted`, `modified`, `is_published` and `is_featured`. The flags default to false, so a new review is a draft. Neither stamp has a database default; the caller sets both. PostgreSQL stores them as `timestamp without time zone`, as it does `SiteSetting.Updated`, in both the migration context and the runtime `ContentContext`.
* `photo_asset_id` is an optional foreign key to `content.asset`, `fk_review_photoassetid__asset_assetid`, with no navigation property; deleting the asset sets it to null in the database.
* Added a `ReviewTag` join, `content.review_tag` (`id`, `review_id`, `tag_id`), modelled on `ArticleTag`, with both foreign keys configured and named (`fk_reviewtags_reviewid__review_reviewid`, `fk_reviewtags_tagid__tag_tagid`) so no provider maps either by convention. Both cascade in the database, so deleting a review deletes its tag rows. The rows are `[JsonIgnore]`; the public shape is the `[NotMapped]` `Review.Tags` string array projected from them.
* Static helpers keep a controller thin: `Review.ByPermalinkOrId` reads a whole number as an id and anything else as a permalink, includes the tags, and can leave drafts out; `Review.List` is the list query, with tags, newest `posted` first and then by descending id, leaving drafts out unless asked; `Review.IsPermalinkValid` checks that no other review uses a permalink; `Review.GeneratePermalink` builds one from a title with the rule `NewsController.Add` uses; `Review.EnsurePermalink` fills a missing or blank permalink from the title with it, so a create and an update alike leave a review with a page; `Review.Normalize` trims the takes as `Article.NormalizeTake` does and turns a blank permalink or product URL into null; and `Review.Validate` returns the first problem as a short sentence for a 400 response, or null. It requires the title and product name, enforces every maximum length, refuses a permalink that is only digits (a GET would read it as an id), a product URL that is not an absolute http or https address, and a score outside 1 to 10, and checks the takes with `Article.ValidateTake`.
* Added the `Reviews` and `ReviewTags` DbSets to `IContentContext`, `ContentContext` and the three provider contexts, and one provider-neutral `ReviewConfiguration` and `ReviewTagConfiguration`, applied where the other neutral configurations are. Migrations `Reviews` added for Microsoft, PostgreSql, and Sqlite; each creates the two tables with their indexes and foreign keys, and nothing else. This is additive, so not a breaking change. No security roles are added; reviews are meant to use the News roles.
* Tests cover the migration on an empty Sqlite database, the three snapshots against their models, a round trip of every field, the unique permalink, `ByPermalinkOrId`, `List`, the permalink rules, generation and `EnsurePermalink`, every validation rule, the tag and photo deletes, the PostgreSQL stamp types and the JSON shape under `ReferenceHandler.Preserve`.

## 2026-10-15 - article takes and meta

* Version 1.3.2, a patch release in the 1.3 line; the csproj version is unchanged and CI bumps the patch when it publishes.
* Added two takes to `Article`, `EddieTake` and `DruTake`, stored as `eddie_take` and `dru_take` on `content.article`: optional, nullable columns of at most 280 characters. They are named for the two Two Wheel Junction riders and are unused by other sites; existing rows read as null. `Article.NormalizeTake` trims a take and turns a blank one into null, and `Article.ValidateTake` refuses a take that, once trimmed, holds a line break (CR or LF) or is longer than `Article.TakeMaxLength` (280, counted as `string.Length`, which is what SQL Server's `nvarchar(280)` counts).
* Added an `ArticleMeta` entity for an article's SEO meta tags, mirroring `ProductMeta`: `content.article_meta` with a `bigint` identity `id`, the owning `article_id`, a required `property` of at most 255 characters and an optional `value` of at most 2048, a unique index on `(article_id, property)`, and a foreign key `fk_articlemeta_articleid__article_articleid` that cascades in the database, so deleting an article deletes its meta. One provider-neutral `ArticleMetaConfiguration` maps it with both sides of the relationship named. The rows are `[JsonIgnore]`; the public shape is the `[NotMapped]` `Article.Meta` dictionary projected from them, ordered by property so the JSON is stable, and `{}` when there are none. `Article.ByPermalinkOrId` now includes the rows; a list must `Include("ArticleMeta")` itself for `Meta` to be populated. Optional and unused by other sites.
* `Article.SubmittedMeta` is what a request body put in `Meta`, and is null when the body omitted `meta` or sent null; `Meta` itself never reads as null. `Article.ValidateMeta` and `Article.ValidateTakesAndMeta` return a message for a 400 response, or null. Meta keys are trimmed and must be non-empty, at most 255 characters, and unique without regard to case, because SQL Server's unique index ignores case; values must be at most 2048 characters. `Article.ApplyMeta` makes an article's rows match a submitted set: it adds and updates rows and removes the missing ones through the context so they are deleted, `{}` clears them all, and null changes nothing. It matches keys to rows without regard to case, stores a null value as an empty string, and silently drops keys starting with `$`, because the host's `ReferenceHandler.Preserve` puts `$id` in the serialised dictionary and an Admin app may send it back.
* Added the `ArticleMeta` DbSet to `IContentContext`, `ContentContext` and the three provider contexts, as `SiteSettings` was in 1.3.1. Migrations `ArticleTakesAndMeta` added for Microsoft, PostgreSql, and Sqlite; each adds the two columns and the `article_meta` table with its index and foreign key, and nothing else. This is additive, so not a breaking change.
* Tests cover the new migration on an empty Sqlite database, the SQL Server and PostgreSQL snapshots against their models, takes and meta round trips, the unique index, the cascade, `ByPermalinkOrId` including the meta, the `Meta` projection and JSON shape (including a `ReferenceHandler.Preserve` response sent back), and the take and meta rules.
* Added `Product.SubmittedMeta`, the counterpart of `Article.SubmittedMeta`: what a request body put in `Product.Meta`, and null when the body omitted `meta` or sent null. `Product.Meta` caches its projection of the `ProductMeta` rows the first time it is read, and model validation and serialisation read it, so on a body without meta it reads as an empty set rather than null; `SubmittedMeta` tracks the assignment separately, so reading `Meta` never makes it non-null. `Meta`'s projection, caching and JSON are unchanged, `SubmittedMeta` is not serialised, and there is no schema change. Tests cover bodies without `meta`, with null, `{}` and an object, a `ReferenceHandler.Preserve` response sent back, `Meta` read first, the projection and JSON shape, and the meta step of the controller's update on Sqlite.

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
* From this release the package is published to the Satori feed only,
  `https://nuget.satori-assoc.com/v3/index.json`, and no longer to
  nuget.org; the publish workflow's nuget.org push step is removed.
  Consumers need that feed as a package source to restore 1.3.1 and
  later.

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
