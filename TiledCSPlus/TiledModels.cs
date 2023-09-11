namespace TiledCSPlus
{
    /// <summary>
    /// Represents a vector with 2 points
    /// </summary>
    public struct Vector2
    {
        /// <summary>
        /// X parameter of the vector
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y parameter of the vector
        /// </summary>
        public float Y { get; set; }

        public Vector2(float x = 0, float y = 0)
        {
            this.X = x;
            this.Y = y;
        }
    }

    /// <summary>
    /// Represents a size
    /// </summary>
    public struct Size
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public Size(float width = 0, float height = 0)
        {
            this.Width = width;
            this.Height = height;
        }
    }

    /// <summary>
    /// Represents a color in RGBA format
    /// </summary>
    public class Color
    {
        /// <summary>
        /// Red channel of the color
        /// </summary>
        public byte R { get; set; }

        /// <summary>
        /// Green channel of the color
        /// </summary>
        public byte G { get; set; }

        /// <summary>
        /// Blue channel of the color
        /// </summary>
        public byte B { get; set; }

        /// <summary>
        /// Alpha channel of the color
        /// </summary>
        public byte A { get; set; }

        public Color(byte r, byte g, byte b, byte a = 255)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }
    }

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
        /// Total horizontal and vertical tiles
        /// </summary>
        public Size Size { get; internal set; }

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
        public Vector2 Parrallax { get; internal set; }

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
        /// The object type if defined. Null if none was set.
        /// </summary>
        public string Type { get; internal set; }

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
        public Size Size { get; internal set; }

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
        /// If an object was set to a point shape, this property will be set
        /// </summary>
        public TiledPoint Point { get; internal set; }

        /// <summary>
        /// If an object was set to an ellipse shape, this property will be set
        /// </summary>
        public TiledEllipse Ellipse { get; internal set; }
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
    /// Represents a point shape
    /// </summary>
    public class TiledPoint
    {
    }

    /// <summary>
    /// Represents an ellipse shape
    /// </summary>
    public class TiledEllipse
    {
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
        /// The custom tile type, set by the user
        /// </summary>
        public string Type { get; internal set; }

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
        /// The image size
        /// </summary>
        public Size Size { get; internal set; }

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
        /// Size of rectangle
        /// </summary>
        public Size Size { get; internal set; }

        /// <summary>
        /// Position of rectangle
        /// </summary>
        public Vector2 Position { get; internal set; }
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
        /// The chunk's position
        /// </summary>
        public Vector2 Position { get; internal set; }

        /// <summary>
        /// The chunk's position
        /// </summary>
        public Size Size { get; internal set; }
        
        /// <summary>
        /// The chunk's data is similar to the data array in the TiledLayer class
        /// </summary>
        public int[] Data { get; internal set; }
        
        /// <summary>
        /// The chunk's data rotation flags are similar to the data rotation flags array in the TiledLayer class
        /// </summary>
        public byte[] DataRotationFlags { get; internal set; }
    }
}