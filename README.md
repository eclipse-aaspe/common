# aaspe-common

AASPE Common Components Library for AASPE and AASX-Server  
AASPE AasCore Library /V3.0 AAS Specification/ for AASPE and AASX-Server  

## included components and namespaces

Aaspe.Common (aaspe-common)

- jsoncanonicalizer
    - Org.Webpki.Es6NumberSerialization
- es6numberserializer
    - Org.Webpki.JsonCanonicalizer
- SSIExtension
    -SSIExtension

Aaspe.AasCore (aaspe-aascore)

## usage

### building a nuget

See [documentation.md]() (tbd)

### using a nuget

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

1. R-click the project (e.g. `AasxPackageLogic`) > Manage nugets

1. If using a local nuget: change `Package source` to the new source `local nugets`

1. Install the nuget.

Enjoy.
