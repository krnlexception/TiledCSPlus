using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace TiledCSPlus
{
    /// <summary>
    /// Represents an element within the Tilesets array of a TiledMap object
    /// </summary>
    public class TiledMapTileset
    {
        /// <summary>
        /// The first gid defines which gid matches the tile with source vector 0,0. Is used to determine which tileset belongs to which gid
        /// </summary>
        public int FirstGid { get; internal set; }

        /// <summary>
        /// Returns true if tileset is embedded in map
        /// </summary>
        public bool IsTilesetEmbedded { get; internal set; }

        /// <summary>
        /// The tsx file path as defined in the map file itself
        /// </summary>
        public string Source { get; internal set; }
    }

    /// <summary>
    /// Represents a property object in both tilesets, maps, layers and objects. Values are all in string but you can use the 'type' property for conversions
    /// </summary>
    public class TiledProperty
    {
        /// <summary>
        /// The property name or key in string format
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The property type as used in Tiled. Can be bool, number, string, ...
        /// </summary>
        public TiledPropertyType Type { get; internal set; }

        /// <summary>
        /// The value in string format
        /// </summary>
        public string Value { get; internal set; }
    }

    /// <summary>
    /// Represents a tile layer as well as an object layer within a tile map
    /// </summary>
    public class TiledLayer
    {
        /// <summary>
        /// The layer id
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// The layer name
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Total horizontal tiles
        /// </summary>
        public int Width { get; internal set; }

        /// <summary>
        /// Total vertical tiles
        /// </summary>
        public int Height { get; internal set; }

        /// <summary>
        /// The layer type.
        /// </summary>
        public TiledLayerType Type { get; internal set; }

        /// <summary>
        /// The tint color set by the user in Color class
        /// </summary>
        public Color TintColor { get; internal set; }

        /// <summary>
        /// Defines if the layer is visible in the editor
        /// </summary>
        public bool Visible { get; internal set; }

        /// <summary>
        /// Is true when the layer is locked
        /// </summary>
        public bool Locked { get; internal set; }

        /// <summary>
        /// Layer offset
        /// </summary>
        public Vector2 Offset { get; internal set; }

        /// <summary>
        /// Parallax position
        /// </summary>
        public Vector2 Parallax { get; internal set; }

        /// <summary>
        /// The layer opacity
        /// </summary>
        public float Opacity { get; internal set; }

        /// <summary>
        /// The layer class
        /// </summary>
        public string Class { get; internal set; }

        /// <summary>
        /// An int array of gid numbers which define which tile is being used where. The length of the array equals the layer width * the layer height. Is null when the layer is not a tilelayer.
        /// </summary>
        public int[] Data { get; internal set; }

        /// <summary>
        /// A parallel array to data which stores the rotation flags of the tile.
        /// Bit 3 is horizontal flip,
        /// bit 2 is vertical flip, and
        /// bit 1 is (anti) diagonal flip.
        /// Is null when the layer is not a tilelayer.
        /// </summary>
        public byte[] DataRotationFlags { get; internal set; }

        /// <summary>
        /// The list of objects in case of an objectgroup layer. Is null when the layer has no objects.
        /// </summary>
        public TiledObject[] Objects { get; internal set; }

        /// <summary>
        /// The layer properties if set
        /// </summary>
        public TiledProperty[] Properties { get; internal set; }

        /// <summary>
        /// The image the layer represents when the layer is an image layer
        /// </summary>
        public TiledImage Image { get; internal set; }

        /// <summary>
        /// The chunks of data when the map is infinite
        /// </summary>
        public TiledChunk[] Chunks { get; internal set; }
    }

    /// <summary>
    /// Represents an tiled object defined in object layers and tiles
    /// </summary>
    public class TiledObject
    {
        /// <summary>
        /// The object id
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// The object's name
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The object's class
        /// </summary>
        public string Class { get; internal set; }

        /// <summary>
        /// Object's position in pixels
        /// </summary>
        public Vector2 Position { get; internal set; }

        /// <summary>
        /// The object's rotation
        /// </summary>
        public float Rotation { get; internal set; }

        /// <summary>
        /// Object's size in pixels
        /// </summary>
        public Vector2 Size { get; internal set; }

        /// <summary>
        /// Object type
        /// </summary>
        public TiledObjectType Type { get; internal set; }

        /// <summary>
        /// The tileset gid when the object is linked to a tile
        /// </summary>
        public int Gid { get; internal set; }

        /// <summary>
        /// A byte which stores the rotation flags of the tile linked to the object's gid.
        /// Bit 3 is horizontal flip,
        /// bit 2 is vertical flip, and
        /// bit 1 is (anti) diagonal flip.
        /// Is null when the layer is not a tilelayer.
        /// </summary>
        public byte DataRotationFlag { get; internal set; }

        /// <summary>
        /// An array of properties. Is null if none were defined.
        /// </summary>
        public TiledProperty[] Properties { get; internal set; }

        /// <summary>
        /// If an object was set to a polygon shape, this property will be set and can be used to access the polygon's data
        /// </summary>
        public TiledPolygon Polygon { get; internal set; }

        /// <summary>
        /// If an object was set to a polyline shape, this property will be set and can be used to access the polyline's data
        /// </summary>
        public TiledPolyline Polyline { get; internal set; }
    }

    /// <summary>
    /// Represents a polygon shape
    /// </summary>
    public class TiledPolygon
    {
        /// <summary>
        /// The array of vertices represented in Vector2 format.
        /// </summary>
        public Vector2[] Points { get; internal set; }
    }

    /// <summary>
    /// Represents a poly line shape
    /// </summary>
    public class TiledPolyline
    {
        /// <summary>
        /// The array of vertices where each two elements represent an x and y position. Like 'x,y,x,y,x,y,x,y'.
        /// </summary>
        public Vector2[] Points { get; internal set; }
    }

    /// <summary>
    /// Represents a tile within a tileset
    /// </summary>
    /// <remarks>These are not defined for all tiles within a tileset, only the ones with properties, terrains and animations.</remarks>
    public class TiledTile
    {
        /// <summary>
        /// The tile id
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// The custom tile class, set by the user
        /// </summary>
        public string Class { get; internal set; }

        /// <summary>
        /// The terrain definitions as int array. These are indices indicating what part of a terrain and which terrain this tile represents.
        /// </summary>
        /// <remarks>In the map file empty space is used to indicate null or no value. However, since it is an int array I needed something so I decided to replace empty values with -1.</remarks>
        public int[] Terrain { get; internal set; }

        /// <summary>
        /// An array of properties. Is null if none were defined.
        /// </summary>
        public TiledProperty[] Properties { get; internal set; }

        /// <summary>
        /// An array of tile animations. Is null if none were defined.
        /// </summary>
        public TiledTileAnimation[] Animations { get; internal set; }

        /// <summary>
        /// An array of tile objects created using the tile collision editor
        /// </summary>
        public TiledObject[] Objects { get; internal set; }

        /// <summary>
        /// The individual tile image
        /// </summary>
        public TiledImage Image { get; internal set; }
    }

    /// <summary>
    /// Represents an image
    /// </summary>
    public class TiledImage
    {
        /// <summary>
        /// The image width
        /// </summary>
        public int Width { get; internal set; }

        /// <summary>
        /// The image height
        /// </summary>
        public int Height { get; internal set; }

        /// <summary>
        /// The image source path
        /// </summary>
        public string Source { get; internal set; }
    }

    /// <summary>
    /// Represents a tile animation. Tile animations are a group of tiles which act as frames for an animation.
    /// </summary>
    public class TiledTileAnimation
    {
        /// <summary>
        /// The tile id within a tileset
        /// </summary>
        public int TileId { get; internal set; }

        /// <summary>
        /// The duration in miliseconds
        /// </summary>
        public int Duration { get; internal set; }
    }

    /// <summary>
    /// Used as data type for the GetSourceRect method. Represents basically a rectangle.
    /// </summary>
    public class TiledSourceRect
    {
        /// <summary>
        /// The x position in pixels from the tile location in the source image
        /// </summary>
        public int X { get; internal set; }

        /// <summary>
        /// The y position in pixels from the tile location in the source image
        /// </summary>
        public int Y { get; internal set; }

        /// <summary>
        /// The width in pixels from the tile in the source image
        /// </summary>
        public int Width { get; internal set; }

        /// <summary>
        /// The height in pixels from the tile in the source image
        /// </summary>
        public int Height { get; internal set; }
    }

    /// <summary>
    /// Represents a layer or object group
    /// </summary>
    public class TiledGroup
    {
        /// <summary>
        /// The group's id
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// The group's name
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The group's visibility
        /// </summary>
        public bool Visible { get; internal set; }

        /// <summary>
        /// The group's locked state
        /// </summary>
        public bool Locked { get; internal set; }

        /// <summary>
        /// The group's user properties
        /// </summary>
        public TiledProperty[] Properties { get; internal set; }

        /// <summary>
        /// The group's layers
        /// </summary>
        public TiledLayer[] Layers { get; internal set; }

        /// <summary>
        /// The group's objects
        /// </summary>
        public TiledObject[] Objects { get; internal set; }

        /// <summary>
        /// The group's subgroups
        /// </summary>
        public TiledGroup[] Groups { get; internal set; }
    }

    /// <summary>
    /// Represents a tile layer chunk when the map is infinite
    /// </summary>
    public class TiledChunk
    {
        /// <summary>
        /// The chunk's x position
        /// </summary>
        public int X { get; internal set; }

        /// <summary>
        /// The chunk's y position
        /// </summary>
        public int Y { get; internal set; }

        /// <summary>
        /// The chunk's width
        /// </summary>
        public int Width { get; internal set; }

        /// <summary>
        /// The chunk's height
        /// </summary>
        public int Height { get; internal set; }

        /// <summary>
        /// The chunk's data is similar to the data array in the TiledLayer class
        /// </summary>
        public int[] Data { get; internal set; }

        /// <summary>
        /// The chunk's data rotation flags are similar to the data rotation flags array in the TiledLayer class
        /// </summary>
        public byte[] DataRotationFlags { get; internal set; }
    }

    /// <summary>
    /// Represents terrain set
    /// </summary>
    public class TiledTerrainSet
    {
        /// <summary>
        /// The name of terrain set
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The class of terrain set
        /// </summary>
        public string Class { get; internal set; }

        /// <summary>
        /// The tile ID of the tile representing this terrain set
        /// </summary>
        public int Tile { get; internal set; }

        /// <summary>
        /// The terrain set's colors
        /// </summary>
        public TiledTerrainSetColor[] TerrainSetColors { get; internal set; }

        /// <summary>
        /// The terrain set's tiles
        /// </summary>
        public Dictionary<int, TiledTerrainSetTile> TerrainSetTiles { get; internal set; }

        /// <summary>
        /// The terrain set's properties
        /// </summary>
        public TiledProperty[] Properties { get; internal set; }

        /// <summary>
        /// The type of terrain set
        /// </summary>
        public TiledTerrainSetType Type { get; internal set; }
    }

    /// <summary>
    /// Represents terrain set color
    /// </summary>
    public class TiledTerrainSetColor
    {
        /// <summary>
        /// The terrain set color's color
        /// </summary>
        public Color Color { get; internal set; }

        /// <summary>
        /// The terrain set's color name
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The terrain set's color class
        /// </summary>
        public string Class { get; internal set; }

        /// <summary>
        /// The tile ID chosen as color icon
        /// </summary>
        public int Tile { get; internal set; }

        /// <summary>
        /// The terrain set's color probability
        /// </summary>
        public float Probability { get; internal set; }

        /// <summary>
        /// The terrain set's color properties
        /// </summary>
        public TiledProperty[] Properties { get; internal set; }
    }

    /// <summary>
    /// Represents terrain set's single tile
    /// </summary>
    public class TiledTerrainSetTile
    {
        /// <summary>
        /// Top color of terrain set tile
        /// </summary>
        public int Top { get; internal set; }

        /// <summary>
        /// Top right color of terrain set tile
        /// </summary>
        public int TopRight { get; internal set; }

        /// <summary>
        /// Right color of terrain set tile
        /// </summary>
        public int Right { get; internal set; }

        /// <summary>
        /// Bottom right color of terrain set tile
        /// </summary>
        public int BottomRight { get; internal set; }

        /// <summary>
        /// Bottom color of terrain set tile
        /// </summary>
        public int Bottom { get; internal set; }

        /// <summary>
        /// Bottom left color of terrain set tile
        /// </summary>
        public int BottomLeft { get; internal set; }

        /// <summary>
        /// Left color of terrain set tile
        /// </summary>
        public int Left { get; internal set; }

        /// <summary>
        /// Top left color of terrain set tile
        /// </summary>
        public int TopLeft { get; internal set; }
    }

    /// <summary>
    /// Represents object types in Tiled
    /// </summary>
    public enum TiledObjectType
    {
        /// <summary>
        /// Point object type
        /// </summary>
        Point,

        /// <summary>
        /// Eclipse object type
        /// </summary>
        Eclipse,

        /// <summary>
        /// Polygon object type
        /// </summary>
        Polygon,

        /// <summary>
        /// Polyline object type (used when polygon object is not closed)
        /// </summary>
        Polyline,

        /// <summary>
        /// Tile object type
        /// </summary>
        Tile,

        /// <summary>
        /// Rectangular object type
        /// </summary>
        Rectangular
    }
}