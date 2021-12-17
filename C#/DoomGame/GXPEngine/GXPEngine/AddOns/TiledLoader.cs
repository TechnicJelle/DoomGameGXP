using System;
using GXPEngine;
using GXPEngine.Core;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace TiledMapParser {
	/// <summary>
	/// A class for automatically creating GXPEngine sprites from Tiled files.
	/// </summary>
	public class TiledLoader {
		public readonly Map map;

		/// <summary>
		/// All generated objects will be added as child of this object. 
		/// If null, game will be used as parent object.
		/// </summary>
		public GameObject rootObject;
		/// <summary>
		/// Whether the newly generated sprites will have colliders.
		/// </summary>
		public bool addColliders;
		/// <summary>
		/// Set this to true if this TiledLoader should automatically create instances of a custom classes,
		/// when LoadObjectLayers is called.
		/// In this case, the [Type] value of the Tiled object is used as class name (case sensitive!).
		/// For image objects this requires that your class inherits from AnimationSprite, and 
		///  has a constructor with parameters (string imageFile, int columns, int rows, TiledObject object).
		/// For shape objects this requires that your class inherits from Sprite, and 
		///  has a constructor with parameters (TiledObject object). 
		/// For text objects this option is ignored.
		/// Your class should be in the global namespace.
		/// If false, basic AnimationSprites are created.
		/// </summary>
		public bool autoInstance;
		/// <summary>
		/// The x origin for all newly generated sprites.
		/// </summary>
		public float defaultOriginX;
		/// <summary>
		/// The y origin for all newly generated sprites.
		/// </summary>
		public float defaultOriginY;
		/// <summary>
		/// Whether text elements will use high quality antialiasing 
		/// (at the cost of about 4x more memory usage).
		/// </summary>
		public bool highQualityText;

		public delegate void ObjectCreateCallback(Sprite sprite, TiledObject obj);
		/// <summary>
		/// This event is fired for each Tiled Object while reading object layers from the Tiled file.
		/// It is fired whenever an AnimationSprite or text element (EasyDraw) is generated from a Tiled object.
		/// It is fired with sprite=null whenever a Tiled object without text or image data is read, or 
		/// when a Tiled object with one of the types in the manualObjects list is read (use this 
		/// in combination with SetPositionRotationScaleOrigin to create your own objects such as a player
		/// that inherits from Sprite).
		/// </summary>
		public event ObjectCreateCallback OnObjectCreated;

		/// <summary>
		/// Returns the number of tile layers in the loaded map.
		/// </summary>
		public int NumTileLayers {
			get {
				if (map.Layers==null) {
					return 0;
				} else {
					return map.Layers.Length;
				}
			}
		}
		/// <summary>
		/// Returns the number of image layers in the loaded map.
		/// </summary>
		public int NumImageLayers {
			get {
				if (map.ImageLayers==null) {
					return 0;
				} else {
					return map.ImageLayers.Length;
				}
			}
		}
		/// <summary>
		/// Returns the number of object groups in the loaded map.
		/// </summary>
		public int NumObjectGroups {
			get {
				if (map.ObjectGroups==null) {
					return 0;
				} else {
					return map.ObjectGroups.Length;
				}
			}
		}


		string _foldername;
		List<string> _manualObjects;
		Assembly _callingAssembly;

		/// <summary>
		/// Creates a new TiledLoader and loads the Tiled file given by filename. 
		/// (The path should be relative to the current folder, typically bin/Debug or bin/Release.)
		/// Sets various public state variables for the Tiled loader.
		/// Call LoadTileLayers, LoadImageLayers and LoadObjectLayers to create GXPEngine sprites from the
		/// layers in the given Tiled file.
		/// </summary>
		/// <param name="filename">the name of the Tiled file (including .tmx extension).</param>
		/// <param name="rootObject">start value for rootObject.</param>
		/// <param name="addColliders">start value for addColliders.</param>
		/// <param name="defaultOriginX">start value for defaultOriginX.</param>
		/// <param name="defaultOriginY">start value for defaultOriginY.</param>
		/// <param name="highQualityText">start value for highQualityText.</param>
		/// <param name="autoInstance">start value for autoInstance.</param>
		/// <param name="callback">A method to be called by the OnObjectCreated event.</param>
		public TiledLoader(string filename,  
			GameObject rootObject = null, bool addColliders = true,
			float defaultOriginX = 0.5f, float defaultOriginY = 0.5f, 
			bool highQualityText = true, bool autoInstance=false, ObjectCreateCallback callback=null) 
		{

			_foldername=Path.GetDirectoryName(filename);

			map = MapParser.ReadMap(filename);

			this.rootObject=rootObject;
			if (this.rootObject==null) {
				this.rootObject=Game.main;
			}
			this.addColliders = addColliders;
			this.autoInstance = autoInstance;
			this.defaultOriginX = defaultOriginX;
			this.defaultOriginY = defaultOriginY;
			this.highQualityText=highQualityText;
			if (callback!=null) {
				OnObjectCreated += callback;
			}
			_manualObjects=new List<string>();
			_callingAssembly = Assembly.GetCallingAssembly();
		}

		/// <summary>
		/// Register names to the list of manually created objects.
		/// When a Tiled Object with such a type is found, this loader will not create a sprite, 
		/// but only fire an event, such that you can create a specific type of game object yourself.
		/// </summary>
		/// <param name="typeName"></param>
		public void AddManualType(params string[] typeNames) {
			foreach (string typeName in typeNames) {
				_manualObjects.Add(typeName);
			}
		}

		//--------------------------------------------------------------------------------------------
		// REGION: public static utility methods, related to creating and placing GXPEngine objects
		// based on data from Tiled Objects.
		//--------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates an EasyDraw for displaying text, based on the configuration parameters of a 
		/// Tiled object. (Including font, text alignment, color)
		/// </summary>
		/// <param name="obj">The Tiled (text) object</param>
		/// <returns></returns>
		public static EasyDraw CreateTextField(TiledObject obj, bool addCollider = true, bool highQualityText = true) {
			float scaleMultiplier = highQualityText ? 2 : 1; // 1=as is. 2=better antialiasing, but 4 x size
			EasyDraw message = new EasyDraw((int)Mathf.Ceiling(obj.Width * scaleMultiplier), (int)Mathf.Ceiling(obj.Height * scaleMultiplier), addCollider);
			// TODO: Cache fonts?

			// Set Font:
			FontStyle f = FontStyle.Regular;
			if (obj.textField.bold == 1 && obj.textField.italic == 1) {
				f = FontStyle.Bold | FontStyle.Italic;
			} else if (obj.textField.bold == 0 && obj.textField.italic == 1) {
				f = FontStyle.Italic;
			} else if (obj.textField.bold == 1 && obj.textField.italic == 0) {
				f = FontStyle.Bold;
			}
			message.TextFont(new Font(obj.textField.font, Mathf.Round(obj.textField.fontSize * scaleMultiplier), f, GraphicsUnit.Pixel));
			message.graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit; //AntiAlias;

			// Set text alignment:
			message.TextAlign(
				obj.textField.horizontalAlign == "left" ? CenterMode.Min : (obj.textField.horizontalAlign == "right" ? CenterMode.Max : CenterMode.Center),
				obj.textField.verticalAlign == "top" ? CenterMode.Min : (obj.textField.verticalAlign == "bottom" ? CenterMode.Max : CenterMode.Center)
			);

			// Set color:
			uint col = obj.textField.Color;
			message.Fill(Color.FromArgb((int)(col&255), (int)((col>>24)&255), (int)((col>>16)&255), (int)((col>>8)&255)), (int)(col&255));

			return message;
		}

		/// <summary>
		/// Draws a given textstring to an EasyDraw, following Tiled's text alignment conventions.
		/// (It uses the text alignment settings of the textCanvas)
		/// </summary>
		/// <param name="textCanvas">the EasyDraw for displaying text, for instance as created by CreateTextField</param>
		/// <param name="text">The text to display. Note that text is not automatically wrapped, so include
		/// manual newline symbols (\n)</param>
		public static void DrawText(EasyDraw textCanvas, string text) {
			float pixelWidth = textCanvas.width / textCanvas.scaleX;
			float pixelHeight = textCanvas.height / textCanvas.scaleY;
			// Set Text alignment:
			float textX =
				textCanvas.HorizontalTextAlign==CenterMode.Min ? 0 :
				(textCanvas.HorizontalTextAlign==CenterMode.Max ? pixelWidth : pixelWidth/2f);
			float textY =
				textCanvas.VerticalTextAlign==CenterMode.Min ? 0 :
				(textCanvas.VerticalTextAlign==CenterMode.Max ? pixelHeight : pixelHeight/2f);

			textCanvas.Text(text, textX, textY);
		}

		/// <summary>
		/// Changes the origin of a (possibly scaled and rotated) sprite, without changing the position.
		/// </summary>
		/// <param name="spr">The sprite</param>
		/// <param name="newOriginRelativeX">The new x origin, normalized (typically a value between 0 and 1)</param>
		/// <param name="newOriginRelativeY">The new y origin, normalized (typically a value between 0 and 1)</param>
		/// <param name="oldOriginX">The old x origin, normalized (typically a value between 0 and 1; GXP default is 0)</param>
		/// <param name="oldOriginY">The old y origin, normalized (typically a value between 0 and 1; GXP default is 0)</param>
		public static void ChangeOrigin(Sprite spr, float newOriginRelativeX = 0.5f, float newOriginRelativeY = 0.5f, float oldOriginX = 0, float oldOriginY = 0) {
			GameObject parent = spr.parent;
			spr.parent=null; // unparent, to avoid recursive TransformPoint

			float imageWidth = spr.width/spr.scaleX;
			float imageHeight = spr.height/spr.scaleY;
			Vector2 newCenter = spr.TransformPoint((newOriginRelativeX-oldOriginX) * imageWidth, (newOriginRelativeY-oldOriginY) * imageHeight);
			//Console.WriteLine("Setting origin. x={0} y={1} w={2} h={3} r={4} tx={5} ty={6}. nO={7},{8}",
			//	spr.x,spr.y,spr.width,spr.height,spr.rotation,newCenter.x,newCenter.y,newOriginRelativeX,newOriginRelativeY);
			spr.x=newCenter.x;
			spr.y=newCenter.y;
			spr.SetOrigin(newOriginRelativeX * imageWidth, newOriginRelativeY * imageHeight);

			spr.parent=parent;
		}

		/// <summary>
		/// Sets the position, rotation and scale of a sprite based on the data in a TiledObject,
		/// and then sets the origin of the sprite.
		/// </summary>
		/// <param name="newSprite">The sprite to be changed</param>
		/// <param name="obj">The Tiled Object with the transform data</param>
		/// <param name="normalizedOriginX">the new x-origin, normalized (=typically between 0 and 1)</param>
		/// <param name="normalizedOriginY">the new y-origin, normalized (=typically between 0 and 1)</param>
		public static void SetPositionRotationScaleOrigin(Sprite newSprite, TiledObject obj, float normalizedOriginX = 0.5f, float normalizedOriginY = 0.5f) {
			newSprite.scale=1;
			// SetOrigin according to Tiled's weird and inconsistent conventions:
			float originY = obj.ImageID>=0 ? 1 : 0;
			newSprite.SetOrigin(0, originY * newSprite.height);

			// Set transform data using this origin:
			newSprite.x = obj.X;
			newSprite.y = obj.Y;
			newSprite.rotation = obj.Rotation;
			newSprite.scaleX=obj.Width / newSprite.width;
			newSprite.scaleY=obj.Height / newSprite.height;

			// Set the desired origin:
			ChangeOrigin(newSprite, normalizedOriginX, normalizedOriginY, 0, originY);
		}

		//--------------------------------------------------------------------------------------------
		// REGION: public methods for reading layers from the Tiled file and creating 
		// GXPEngine objects from them. These methods use the state variables from TiledLoader
		// (rootObject, addColliders, defaultOrigins, highQualityText, autoInstance).
		//--------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates animation sprites and text sprites from the given object groups from the loaded Tiled file.
		/// If no object group indices are given, all object groups are loaded.
		/// Uses the current settings from TiledLoader (rootObject, addColliders, defaultOrigins, highQualityText, autoInstance).
		/// Subscribe to the OnObjectCreated event to get a callback for each created sprite.
		/// </summary>
		/// <param name="layerIndices">The indices of the object layers that should be loaded.</param>
		public void LoadObjectGroups(params int[] layerIndices) {
			if (map.ObjectGroups==null)
				return;
			if (layerIndices.Length==0) {
				for (int i = 0; i<map.ObjectGroups.Length; i++) {
					LoadObjectGroup(i,autoInstance);
				}
			} else {
				foreach (int index in layerIndices) {
					LoadObjectGroup(index,autoInstance);
				}
			}
		}

		/// <summary>
		/// Creates animation sprites for the given tile layers from the loaded Tiled file.
		/// If no tile layer indices are given, all tile layers are loaded.
		/// Uses the current settings from TiledLoader (rootObject, addColliders, defaultOrigins).
		/// </summary>
		/// <param name="layerIndices">The indices of the tile layers that should be loaded.</param>
		public void LoadTileLayers(params int[] layerIndices) {
			if (map.Layers==null)
				return;
			if (layerIndices.Length==0) {
				for (int i = 0; i<map.Layers.Length; i++) {
					LoadTileLayer(i);
				}
			} else {
				foreach (int index in layerIndices) {
					LoadTileLayer(index);
				}
			}
		}

		/// <summary>
		/// Creates sprites for the given image layers from the loaded Tiled file.
		/// If no image layer indices are given, all image layers are loaded.
		/// Uses the current settings from TiledLoader (rootObject, addColliders, defaultOrigins).
		/// </summary>
		/// <param name="layerIndices">The indices of the image layers that should be loaded.</param>
		public void LoadImageLayers(params int[] layerIndices) {
			if (map.ImageLayers==null)
				return;
			if (layerIndices.Length==0) {
				for (int i = 0; i<map.ImageLayers.Length; i++) {
					LoadImageLayer(i);
				}
			} else {
				foreach (int index in layerIndices) {
					LoadImageLayer(index);
				}
			}
		}

		//--------------------------------------------------------------------------------------------
		// REGION: private methods (for reading single layers from the Tiled file).
		//--------------------------------------------------------------------------------------------
			   
		void LoadObjectGroup(int index, bool autoInstance=false) {
			if (map.ObjectGroups.Length<=index) return;
			ObjectGroup group = map.ObjectGroups[index];
			if (group.Objects==null)
				return;
			foreach (TiledObject obj in group.Objects) {
				obj.Initialize();
				Sprite newSprite = null;

				if (_manualObjects.Contains(obj.Type)) {
					// Don't create an object, just fire the event and let the user create something.
					//Console.WriteLine("Skipping object because type is in manual list: "+obj);
				} else if (obj.ImageID>=0) { // Create an AnimationSprite
					TileSet tileSet = map.GetTileSet(obj.ImageID);
					if (tileSet==null || tileSet.Image==null)
						throw new Exception("The Tiled map contains unembedded tilesets (.tsx files) - please embed them in the map");

					int frame = obj.ImageID - tileSet.FirstGId;
					//Console.WriteLine("Creating image object: "+obj);

					AnimationSprite anim = null;
					if (autoInstance && obj.Type!=null) {
						try {
							// The simple way:
							//anim = (AnimationSprite)Activator.CreateInstance(Type.GetType(obj.Type),
							//	new object[] { Path.Combine(_foldername, tileSet.Image.FileName), tileSet.Columns, tileSet.Rows, obj });
							// Necessary when using separate assemblies (engine=DLL):
							anim = (AnimationSprite)_callingAssembly.CreateInstance(obj.Type,false,BindingFlags.Default,null,
								new object[] { Path.Combine(_foldername, tileSet.Image.FileName), tileSet.Columns, tileSet.Rows, obj }, null, null);
							if (anim == null) throw new Exception("Class with name " + obj.Type + " not found in the calling assembly. Check namespace?");
						} catch (Exception error) {
							Console.WriteLine("Couldn't automatically create an AnimationSprite object from the Tiled (image) object with ID {0} and type {1}.\n Error: {2}", obj.ID, obj.Type, error.Message);
							// Check the console for more information!
							throw error;
						}
					}
					if (anim==null) {
						anim = new AnimationSprite(Path.Combine(_foldername, tileSet.Image.FileName), tileSet.Columns, tileSet.Rows, -1, false, addColliders);
					}
					anim.Mirror(obj.MirrorX, obj.MirrorY);
					anim.SetFrame(frame);
					newSprite=anim;
				} else if (obj.textField!=null) {
					//Console.WriteLine("Creating text object: "+obj);

					EasyDraw message = CreateTextField(obj, addColliders, highQualityText);

					DrawText(message, obj.textField.text);

					newSprite=message;
				} else {
					//Console.WriteLine("Skipping non-graphical object: "+obj);
					if (autoInstance && obj.Type!="") {
						try {
							// The simple way:
							//newSprite = (Sprite)Activator.CreateInstance(Type.GetType(obj.Type),
							//	new object[] { obj });
							// Necessary when using separate assemblies (engine=DLL):
							newSprite = (Sprite)_callingAssembly.CreateInstance(obj.Type,false,BindingFlags.Default,null,
								new object[] { obj }, null, null);
							if (newSprite == null) throw new Exception("Class with name "+obj.Type+" not found in the calling assembly. Check namespace?");
						} catch (Exception error) {
							Console.WriteLine("Couldn't automatically create a sprite object from the Tiled (shape) object with ID {0} and type {1}.\n Error: {2}", obj.ID, obj.Type, error.Message);
							// Check the console for more information!
							throw error;
						}
					}
				}

				if (newSprite!=null) {
					SetPositionRotationScaleOrigin(newSprite, obj, defaultOriginX, defaultOriginY);
					rootObject.AddChild(newSprite);
				}
				if (OnObjectCreated!=null) {
					OnObjectCreated(newSprite, obj);
				}
			}
		}

		void LoadTileLayer(int index) {
			if (map.Layers.Length<=index) return;
			uint[,] tiles = map.Layers[index].GetTileArrayRaw();
			for (int c = 0; c < tiles.GetLength(0); c++) {
				for (int r = 0; r < tiles.GetLength(1); r++) {
					if (tiles[c, r] == 0)
						continue;
					uint rawTileInfo = tiles[c, r];
					int frame = TiledUtils.GetTileFrame(rawTileInfo);
					TileSet tileSet = map.GetTileSet(frame);
					if (tileSet==null || tileSet.Image==null)
						throw new Exception("The Tiled map contains unembedded tilesets (.tsx files) - please embed them in the map");

					AnimationSprite Tile = new AnimationSprite(
						Path.Combine(_foldername, tileSet.Image.FileName),
						tileSet.Columns, tileSet.Rows,
						-1, false, addColliders
					);
					Tile.SetFrame(frame-tileSet.FirstGId);
					Tile.x = c * map.TileWidth;
					// Adapting to Tiled's weird and inconsistent conventions again:
					Tile.y = r * map.TileHeight - (Tile.height - map.TileHeight);
					ChangeOrigin(Tile, 0.5f, 0.5f);
					Tile.rotation=TiledUtils.GetRotation(rawTileInfo);
					Tile.Mirror(TiledUtils.GetMirrorX(rawTileInfo), false);
					ChangeOrigin(Tile, defaultOriginX, defaultOriginY, 0.5f, 0.5f);

					rootObject.AddChild(Tile);
				}
			}
		}

		void LoadImageLayer(int index) {
			if (map.ImageLayers.Length<=index) return;
			ImageLayer layer = map.ImageLayers[index];
			Console.WriteLine("Loading image: "+layer.Image);
			Sprite image = new Sprite(Path.Combine(_foldername, layer.Image.FileName), false, addColliders);
			image.x=layer.offsetX;
			image.y=layer.offsetY;
			image.alpha=layer.Opacity;

			ChangeOrigin(image, defaultOriginX, defaultOriginY);

			rootObject.AddChild(image);
		}
	}
}