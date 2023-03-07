# Change Log
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/)
and this project adheres to [Semantic Versioning](http://semver.org/).

## [1.0.9]
### Fixed
- Path with space for mac

## [1.0.8]
### Fixed
- Building a mac app with files other than .app
- Path building with space
- Validation for label and changelog

## [1.0.7]
### Added
- Scenes selection control

### Fixed
- Displaying "Build location must be an empty directory." error all the time.

### Changed
- Updating patchkit-tools to version 3.1.7

## [1.0.6]
### Added
- New building option "Include .pdb files"
- Warning about files outside of build entries

### Fixed
- Building when copy PDB files is selected in Build Settings
- Building when files outside of build entries

## [1.0.5]
### Fixed
- Reading API response body with invalid encoding

## [1.0.4]
### Fixed
- New Unity versions compatibility (by changing Newtonsoft.Json to JsonUtility)

### Changed
- Updating patchkit-tools to version 3.1.3

## [1.0.3]
### Fixed
- Handling situation when app has been removed from PatchKit

## [1.0.2]
### Fixed
- Building when using older version of Mono (not BleedingEdge)

## [1.0.1]
### Fixed
- Handling spaces inside path of project and build on Windows
