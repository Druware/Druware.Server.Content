## 2026-07-24

* Floated `Microsoft.AspNetCore.Identity` from the exact 2.3.1 pin to 2.3.*.
  Druware.Server 1.1.16 requires 2.3.11, and the exact pin made that a package
  downgrade, which fails the build outright rather than warning.

## 2026-01-02

* Removed .nuspec
* Updated build scripts to push to both nuget.org and satori.
* Updated Nuget Packages
