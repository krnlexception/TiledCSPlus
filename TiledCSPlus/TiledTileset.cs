using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Xml;

namespace TiledCSPlus
{
    /// <summary>
    /// Represents a Tiled tileset
    /// </summary>
    public class TiledTileset
    {
        /// <summary>
        /// The Tiled version used to create this tileset
        /// </summary>
        public string TiledVersion { get; internal set; }

        /// <summary>
        /// The Tiled version this tileset is compatible with
        /// </summary>
        public string TilesetVersion { get; internal set; }

        /// <summary>
        /// The tileset name
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The tileset class
        /// </summary>
        public string Class { get; internal set; }

        /// <summary>
        /// The tile width in pixels
        /// </summary>
        public int TileWidth { get; internal set; }

        /// <summary>
        /// The tile height in pixels
        /// </summary>
        public int TileHeight { get; internal set; }

        /// <summary>
        /// The total amount of tiles
        /// </summary>
        public int TileCount { get; internal set; }

        /// <summary>
        /// The amount of horizontal tiles
        /// </summary>
        public int Columns { get; internal set; }

        /// <summary>
        /// The image definition used by the tileset
        /// </summary>
        public TiledImage Image { get; internal set; }

        /// <summary>
        /// The amount of spacing between the tiles in pixels
        /// </summary>
        public int Spacing { get; internal set; }

        /// <summary>
        /// The amount of margin between the tiles in pixels
        /// </summary>
        public int Margin { get; internal set; }

        /// <summary>
        /// An array of tile definitions
        /// </summary>
        /// <remarks>Not all tiles within a tileset have definitions. Only those with properties, animations, terrains, ...</remarks>
        public TiledTile[] Tiles { get; internal set; }

        /// <summary>
        /// An array of tileset properties
        /// </summary>
        public TiledProperty[] Properties { get; internal set; }

        /// <summary>
        /// The tile offset in pixels
        /// </summary>
        public Vector2 Offset { get; internal set; }

        /// <summary>
        /// An array of terrain sets
        /// </summary>
        public TiledTerrainSet[] TerrainSets { get; internal set; }

        /// <summary>
        /// Returns an empty instance of TiledTileset
        /// </summary>
        internal TiledTileset()
        {
        }

        /// <summary>
        /// Loads a tileset in TSX format and parses it
        /// </summary>
        /// <param name="path">The file path of the TSX file</param>
        /// <exception cref="TiledException">Thrown when the file could not be found or parsed</exception>
        public TiledTileset(string path)
        {
            // Check the file
            if(!File.Exists(path))
            {
                throw new TiledException($"{path} not found");
            }

            var content = File.ReadAllText(path);

            if(path.EndsWith(".tsx"))
            {
                ParseXml(content);
            }
            else
            {
                throw new TiledException("Unsupported file format");
            }
        }

        /// <summary>
        /// Loads a tileset in TSX format and parses it
        /// </summary>
        /// <param name="stream">The file stream of the TSX file</param>
        /// <exception cref="TiledException">Thrown when the file could not be parsed</exception>
        public TiledTileset(Stream stream)
        {
            var streamReader = new StreamReader(stream);
            var content = streamReader.ReadToEnd();
            ParseXml(content);
        }

        /// <summary>
        /// Can be used to parse the content of a TSX tileset manually instead of loading it using the constructor
        /// </summary>
        /// <param name="xml">The tmx file content as string</param>
        /// <exception cref="TiledException"></exception>
        public void ParseXml(string xml)
        {
            try
            {
                var document = new XmlDocument();
                document.LoadXml(xml);

                var nodeTileset = document.SelectSingleNode("tileset");
                var nodeImage = nodeTileset.SelectSingleNode("image");
                var nodeOffset = nodeTileset.SelectSingleNode("tileoffset");
                var nodesTile = nodeTileset.SelectNodes("tile");
                var nodesProperty = nodeTileset.SelectNodes("properties/property");
                var nodesWangsets = nodeTileset.SelectNodes("wangsets/wangset");

                var attrMargin = nodeTileset.Attributes["margin"];
                var attrSpacing = nodeTileset.Attributes["spacing"];
                var attrClass = nodeTileset.Attributes["class"];

                TiledVersion = nodeTileset.Attributes["tiledversion"]?.Value;
                TilesetVersion = nodeTileset.Attributes["version"]?.Value;
                Name = nodeTileset.Attributes["name"]?.Value;
                TileWidth = int.Parse(nodeTileset.Attributes["tilewidth"].Value);
                TileHeight = int.Parse(nodeTileset.Attributes["tileheight"].Value);
                TileCount = int.Parse(nodeTileset.Attributes["tilecount"].Value);
                Columns = int.Parse(nodeTileset.Attributes["columns"].Value);

                if(attrMargin != null) Margin = int.Parse(nodeTileset.Attributes["margin"].Value);
                if(attrSpacing != null) Spacing = int.Parse(nodeTileset.Attributes["spacing"].Value);
                if(attrClass != null) Class = attrClass.Value;
                if(nodeImage != null) Image = ParseImage(nodeImage);
                if(nodeOffset != null) Offset = ParseOffset(nodeOffset);

                Tiles = ParseTiles(nodesTile);
                Properties = ParseProperties(nodesProperty);
                TerrainSets = ParseTerrainSets(nodesWangsets);
            }
            catch(Exception ex)
            {
                throw new TiledException("An error occurred while trying to parse the Tiled tileset file", ex);
            }
        }

        private Vector2 ParseOffset(XmlNode node)
        {
            var tiledOffset = new Vector2
            {
                X = int.Parse(node.Attributes["x"].Value), Y = int.Parse(node.Attributes["y"].Value)
            };

            return tiledOffset;
        }

        private TiledImage ParseImage(XmlNode node)
        {
            var tiledImage = new TiledImage
            {
                Source = node.Attributes["source"].Value,
                Width = int.Parse(node.Attributes["width"].Value),
                Height = int.Parse(node.Attributes["height"].Value)
            };

            return tiledImage;
        }

        private TiledTileAnimation[] ParseAnimations(XmlNodeList nodeList)
        {
            var result = new List<TiledTileAnimation>();

            foreach(XmlNode node in nodeList)
            {
                var animation = new TiledTileAnimation
                {
                    TileId = int.Parse(node.Attributes["tileid"].Value),
                    Duration = int.Parse(node.Attributes["duration"].Value)
                };

                result.Add(animation);
            }

            return result.ToArray();
        }

        private TiledProperty[] ParseProperties(XmlNodeList nodeList)
        {
            var result = new List<TiledProperty>();

            foreach(XmlNode node in nodeList)
            {
                var attrType = node.Attributes["type"];

                var property = new TiledProperty
                {
                    Name = node.Attributes["name"].Value,
                    Value = node.Attributes["value"]?.Value,
                    Type = TiledPropertyType.String
                };

                if(attrType != null)
                {
                    if(attrType.Value == "bool") property.Type = TiledPropertyType.Bool;
                    if(attrType.Value == "color") property.Type = TiledPropertyType.Color;
                    if(attrType.Value == "file") property.Type = TiledPropertyType.File;
                    if(attrType.Value == "float") property.Type = TiledPropertyType.Float;
                    if(attrType.Value == "int") property.Type = TiledPropertyType.Int;
                    if(attrType.Value == "object") property.Type = TiledPropertyType.Object;
                }

                property.Value ??= node.InnerText;

                result.Add(property);
            }

            return result.ToArray();
        }

        private TiledTile[] ParseTiles(XmlNodeList nodeList)
        {
            var result = new List<TiledTile>();

            foreach(XmlNode node in nodeList)
            {
                var nodesProperty = node.SelectNodes("properties/property");
                var nodesObject = node.SelectNodes("objectgroup/object");
                var nodesAnimation = node.SelectNodes("animation/frame");
                var nodeImage = node.SelectSingleNode("image");

                var tile = new TiledTile
                {
                    Id = int.Parse(node.Attributes["id"].Value),
                    Class = TilesetVersion == "1.9" ? node.Attributes["class"]?.Value : node.Attributes["type"]?.Value,
                    Terrain = node.Attributes["terrain"]?.Value.Split(',').AsIntArray(),
                    Properties = ParseProperties(nodesProperty),
                    Animations = ParseAnimations(nodesAnimation),
                    Objects = ParseObjects(nodesObject)
                };

                if(nodeImage != null)
                {
                    var tileImage = new TiledImage
                    {
                        Width = int.Parse(nodeImage.Attributes["width"].Value),
                        Height = int.Parse(nodeImage.Attributes["height"].Value),
                        Source = nodeImage.Attributes["source"].Value
                    };

                    tile.Image = tileImage;
                }

                result.Add(tile);
            }

            return result.ToArray();
        }

        private TiledObject[] ParseObjects(XmlNodeList nodeList)
        {
            var result = new List<TiledObject>();

            foreach(XmlNode node in nodeList)
            {
                var nodesProperty = node.SelectNodes("properties/property");
                var nodePolygon = node.SelectSingleNode("polygon");
                var nodePoint = node.SelectSingleNode("point");
                var nodeEllipse = node.SelectSingleNode("ellipse");

                var obj = new TiledObject
                {
                    Id = int.Parse(node.Attributes["id"].Value),
                    Name = node.Attributes["name"]?.Value,
                    Class = TilesetVersion == "1.9" ? node.Attributes["class"]?.Value : node.Attributes["type"]?.Value,
                    Gid = int.Parse(node.Attributes["gid"]?.Value ?? "0"),
                    Position = new Vector2(float.Parse(node.Attributes["x"].Value, CultureInfo.InvariantCulture),
                        float.Parse(node.Attributes["y"].Value, CultureInfo.InvariantCulture))
                };

                if(nodesProperty != null)
                {
                    obj.Properties = ParseProperties(nodesProperty);
                }

                if(nodePolygon != null)
                {
                    var points = nodePolygon.Attributes["points"].Value;
                    var vertices = points.Split(' ');

                    var polygon = new TiledPolygon { Points = new Vector2[vertices.Length] };

                    for(var i = 0; i < vertices.Length; i++)
                    {
                        polygon.Points[i] =
                            new Vector2(float.Parse(vertices[i].Split(',')[0], CultureInfo.InvariantCulture),
                                float.Parse(vertices[i].Split(',')[1], CultureInfo.InvariantCulture));
                    }

                    obj.Polygon = polygon;
                }

                if(nodeEllipse != null)
                {
                    obj.Ellipse = new TiledEllipse();
                }

                if(nodePoint != null)
                {
                    obj.Point = new TiledPoint();
                }

                if(node.Attributes["width"] != null || node.Attributes["height"] != null) obj.Size = new Size();
                if(node.Attributes["width"] != null)
                {
                    obj.Size.Width = float.Parse(node.Attributes["width"].Value, CultureInfo.InvariantCulture);
                }

                if(node.Attributes["height"] != null)
                {
                    obj.Size.Height = float.Parse(node.Attributes["height"].Value, CultureInfo.InvariantCulture);
                }

                if(node.Attributes["rotation"] != null)
                {
                    obj.Rotation = float.Parse(node.Attributes["rotation"].Value, CultureInfo.InvariantCulture);
                }

                result.Add(obj);
            }

            return result.ToArray();
        }

        private TiledTerrainSet[] ParseTerrainSets(XmlNodeList nodeList)
        {
            var result = new List<TiledTerrainSet>();
            foreach (XmlNode node in nodeList)
            {
                var nodesProperty = node.SelectNodes("properties/property");
                var nodesColor = node.SelectNodes("wangcolor");
                var nodesTile = node.SelectNodes("wangtile");

                var obj = new TiledTerrainSet()
                {
                    Name = node.Attributes["name"].Value,
                    Tile = Int32.Parse(node.Attributes["tile"].Value),
                    Class = node.Attributes["class"]?.Value
                };

                if (nodesProperty != null)
                {
                    obj.Properties = ParseProperties(nodesProperty);
                }

                switch (node.Attributes["type"].Value)
                {
                    case "corner":
                        obj.Type = TiledTerrainSetType.Corner;
                        break;
                    case "mixed":
                        obj.Type = TiledTerrainSetType.Mixed;
                        break;
                    case "edge":
                        obj.Type = TiledTerrainSetType.Edge;
                        break;
                    default:
                        throw new TiledException($"Unknown terrain set type: {node.Attributes["type"].Value}");
                }

                obj.TerrainSetColors = ParseTerrainSetColors(nodesColor);
                obj.TerrainSetTiles = ParseTerrainSetTiles(nodesTile);

                result.Add(obj);
            }

            return result.ToArray();
        }

        private TiledTerrainSetColor[] ParseTerrainSetColors(XmlNodeList nodeList)
        {
            var result = new List<TiledTerrainSetColor>();
            foreach (XmlNode node in nodeList)
            {
                var nodesProperty = node.SelectNodes("properties/property");

                var obj = new TiledTerrainSetColor()
                {
                    Name = node.Attributes["name"].Value,
                    Class = node.Attributes["class"]?.Value,
                    Tile = Int32.Parse(node.Attributes["tile"].Value),
                    Color = ParseColor(node.Attributes["color"].Value),
                    Probability = float.Parse(node.Attributes["probability"].Value, CultureInfo.InvariantCulture)
                };

                if (nodesProperty != null)
                {
                    obj.Properties = ParseProperties(nodesProperty);
                }

                result.Add(obj);
            }

            return result.ToArray();
        }

        private Dictionary<int, TiledTerrainSetTile> ParseTerrainSetTiles(XmlNodeList nodeList)
        {
            var result = new Dictionary<int, TiledTerrainSetTile>();
            foreach (XmlNode node in nodeList)
            {
                var attrTileid = node.Attributes["tileid"].Value;
                var attrWangid = node.Attributes["wangid"].Value.Split(",");

                var obj = new TiledTerrainSetTile()
                {
                    Top = Int32.Parse(attrWangid[0]),
                    TopRight = Int32.Parse(attrWangid[1]),
                    Right = Int32.Parse(attrWangid[2]),
                    BottomRight = Int32.Parse(attrWangid[3]),
                    Bottom = Int32.Parse(attrWangid[4]),
                    BottomLeft = Int32.Parse(attrWangid[5]),
                    Left = Int32.Parse(attrWangid[6]),
                    TopLeft = Int32.Parse(attrWangid[7])
                };

                result.Add(Int32.Parse(attrTileid), obj);
            }

            return result;
        }

        private Color ParseColor(string hexColor)
        {
            hexColor = hexColor[1..];
            List<byte> color = new();
            for(int i = 0; i < hexColor.Length - 1; i += 2)
            {
                color.Add(Convert.ToByte(hexColor.Substring(i, 2), 16));
            }

            return color.Count == 3
                ? Color.FromArgb(color[0], color[1], color[2])
                : Color.FromArgb(color[0], color[1], color[2], color[3]);
        }
    }
}