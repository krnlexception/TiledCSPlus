using System.Collections.Generic;
using TiledCSPlus;

class Program
{
    public static void Main()
    {
        //load a tilemap
        TiledMap TiledMap = new("assets/tilemap.tmx");

        //load a tileset by loading all used tilesets from a directory
        Dictionary<int, TiledTileset> Tilesets = TiledMap.GetTiledTilesets("assets/");
        //you can load them manually of course, like this
        /* foreach (TiledMapTileset tiledMapTileset in TiledMap.Tilesets)
        {
            if tileset is not embedded into tilemap
            if (!tiledMapTileset.IsTilesetEmbedded)
            {
                load a tileset file using tiledMapTileset.Source as a filename...
                ...using new TiledTileset()
                TiledTileset tiledTileset = new TiledTileset(tiledMapTileset.Source);
                ..or other method that works for you, into a MemoryStream
                MemoryStream ms = new MemoryStream(<loaded tileset file>); //file has to be loaded into a byte array
                TiledTileset tiledTileset = new TiledTileset(ms);

                //then, add into loaded tilesets as usual
                Tilesets.Add(tiledMapTileset.FirstGid, tiledTileset);
            }
        } */

        //get map tileset which some tile belongs to
        TiledMapTileset tileMapTileset = TiledMap.GetTiledMapTileset(TiledMap.Layers[0].Data[0]);
        //now that you have tileset laded into Tilesets dictionary, you can get the tileset by
        TiledTileset tileTileset = Tilesets[tileMapTileset.FirstGid];
        //and now, get the rect for tile's position in the tileset image
        _ = TiledMap.GetSourceRect(tileMapTileset, tileTileset, TiledMap.Layers[0].Data[0]);
    }
}