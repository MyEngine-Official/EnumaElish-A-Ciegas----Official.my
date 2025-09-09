using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;

namespace MonoGameLibrary.Events
{
    public class Collisions : Tilemap
    {
        private readonly Rectangle[] _collisionRects;

        /// <summary>
        /// Creates a new Collisions object.
        /// </summary>
        /// <param name="tileset">The tileset used for the collision map.</param>
        /// <param name="columns">The total number of columns in the collision map.</param>
        /// <param name="rows">The total number of rows in the collision map.</param>
        private Collisions(Tileset tileset, int columns, int rows) : base(tileset, columns, rows)
        {
            _collisionRects = new Rectangle[Count];
        }

        /// <summary>
        /// Gets the array of collision rectangles.
        /// </summary>
        /// <returns>An array of Rectangle objects for collision detection.</returns>
        public Rectangle[] GetCollisionRects()
        {
            return _collisionRects;
        }

        /// <summary>
        /// Creates a new Collisions object based on a tilemap XML configuration file.
        /// </summary>
        /// <param name="content">The content manager used to load the texture for the tileset.</param>
        /// <param name="filename">The path to the XML file, relative to the content root directory.</param>
        /// <returns>The Collisions object created by this method.</returns>
        public static new Collisions FromFile(ContentManager content, string filename)
        {
            string filePath = Path.Combine(content.RootDirectory, filename);

            using (Stream stream = TitleContainer.OpenStream(filePath))
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    XDocument doc = XDocument.Load(reader);
                    XElement root = doc.Root;

                    // The <Tileset> element contains the information about the tileset
                    // used by the tilemap.
                    XElement tilesetElement = root.Element("Tileset");
                    string regionAttribute = tilesetElement.Attribute("region").Value;
                    string[] split = regionAttribute.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    int x = int.Parse(split[0]);
                    int y = int.Parse(split[1]);
                    int width = int.Parse(split[2]);
                    int height = int.Parse(split[3]);

                    int tileWidth = int.Parse(tilesetElement.Attribute("tileWidth").Value);
                    int tileHeight = int.Parse(tilesetElement.Attribute("tileHeight").Value);
                    string contentPath = tilesetElement.Value;

                    // Load the texture 2d at the content path
                    Texture2D texture = content.Load<Texture2D>(contentPath);

                    // Create the texture region from the texture
                    TextureRegion textureRegion = new TextureRegion(texture, x, y, width, height);

                    // Create the tileset using the texture region
                    Tileset tileset = new Tileset(textureRegion, tileWidth, tileHeight);

                    // The <Tiles> element contains lines of strings
                    XElement tilesElement = root.Element("Tiles");
                    string[] rows = tilesElement.Value.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    int columnCount = rows[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;

                    // Create the collisions object
                    Collisions collisions = new Collisions(tileset, columnCount, rows.Length);

                    // Process each row
                    for (int row = 0; row < rows.Length; row++)
                    {
                        // Split the row into individual columns
                        string[] columns = rows[row].Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

                        // Process each column of the current row
                        for (int column = 0; column < columnCount; column++)
                        {
                            // Get the tileset index for this location
                            int tilesetIndex = int.Parse(columns[column]);
                            int index = row * columnCount + column;

                            // If tilesetIndex is not 0, it means it's a collision tile
                            if (tilesetIndex != 0)
                            {
                                int tileX = column * tileWidth;
                                int tileY = row * tileHeight;
                                collisions._collisionRects[index] = new Rectangle(tileX, tileY, tileWidth, tileHeight);
                                // Set the tile index for consistency, though it won't be used for drawing
                                collisions.SetTile(index, tilesetIndex);
                            }
                            else
                            {
                                // For a value of 0 in the XML, set an invalid index to signal no tile
                                // should be drawn, and set the rectangle to an empty one.
                                collisions._collisionRects[index] = Rectangle.Empty;
                                collisions.SetTile(index, -1);
                            }
                        }
                    }
                    return collisions;
                }
            }
        }
    }
}
