# AASX Common Libraries

AASPE Common Components Library for AASPE and AASX-Server  
AASPE AasCore Library /V3.0 AAS Specification/ for AASPE and AASX-Server  

## included components and namespaces

Aaspe.Common (aaspe-common)

- `AasxCsharpLibrary`:
    - `AasxCompatibilityModels`
    - `AasxIntegrationBase`
    - `AdminShell_V20`
    - `AdminShellNS`
    - `AdminShellNS.Exceptions`
    - `AdminShellNS.Extensions`
    - `Extensions`
    - `Extensions.ArrayExtensions`
- `jsoncanonicalizer`
    - `Org.Webpki.Es6NumberSerialization`
- `es6numberserializer`
    - `Org.Webpki.JsonCanonicalizer`
- `SSIExtension`
    - `SSIExtension`

Aaspe.AasCore (aaspe-aascore)

## usage

Nugets can be either pulled from the Ether (nuget.org) or from a local storage.  
Think Release/Debug builds.

#### local nuget

To use a local nuget to a project add a local nuget source.

Tools > Options > Package Manager > Package Sources

Add a new nuget source:

1. Name e.g. `local nugets`

2. Source: the directory where the nuget is,     
    e.g. `D:\eclipse-aaspe\aaspe-common\src\aaspe-common\bin\Debug` is the default pack path

#### nuget.org

[`Aaspe.Commons`](https://www.nuget.org/packages/Aaspe.AasCore) and 
[`Aaspe.AasCore`](https://www.nuget.org/packages/AASPE.Common) are available at nuget.

#### add the nuget to a project

Add the nuget to a project:

1. *(remove the projects contained in the nuget from the repo and dependencies)*

1. R-click the project (e.g. `AasxPackageLogic`) > Manage nugets

1. If using a local nuget: change `Package source` to the new source `local nugets`

1. Install the nuget.

1. *(in case of problems with the nuget installation, try [`Update-Package –reinstall Aaspe.Common`](https://docs.nuget.org/consume/reinstalling-packages))*

1. Enjoy.

## build

See [documentation.md]() (tbd)
