# TiledCSPlus

TiledCSPlus is an extended, and up to date fork of [TiledCS](https://github.com/TheBoneJarmer/TiledCS), a .NET library for loading Tiled maps and tilesets. Currently supports only uncompressed Base64 encoded .TMX maps and .TSX tilesets.

## Planned features
* Support for Tiled 1.9 and 1.10 features.
* Embedded tilesets
* Better layer arrangement (layers will be arranged in order from top to bottom, instead of current tile layers, then object layers, and image layers at the end)

## Does it break compability?
**YES**, practically every field was renamed to be in PascalCase, and few fields were combined into one property eg. `offsetX` and `offsetY` into `Vector2` `offset`. Except those few changes

## License
TiledCSPlus is [licensed under MIT license](LICENSE). TiledCS, the original project, was created by [Ruben Labruyere](https://github.com/TheBoneJarmer), and it was [licensed under MIT license](LICENSE_orig).