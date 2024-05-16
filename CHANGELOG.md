# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Unit-Test project for aaspe-common (@Freezor)

### Changed

- Made internals of aaspe-common visible for aaspe-common-tests (@Freezor)
- Extracted JsonDecoder from JsonConverter (@Freezor)
- Extracted JsonScanner and JsonConverter from JsonDecoder and made them internal (@Freezor)
- Simplified JsonDecoder and made the methods smaller and better maintainable (@Freezor)

### Fixed

- Formatting of .csproj files (@Freezor)

### Updated

- aaspe-common: JetBrains.Annotations from 2022.1.0 to 2023.3.0 (@Freezor)
- aaspe-common: NJsonSchema from 10.8.0 to 11.0.0 (@Freezor)
- aaspe-common: System.IO.Packaging from 6.0.0 to 8.0.0 (@Freezor)
