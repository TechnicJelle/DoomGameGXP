using GXPEngine.Core; // For Vector2

namespace GXPEngine {
	/// <summary>
	/// A Camera gameobject, that owns a rectangular render window, and determines the focal point, rotation and scale
	/// of what's rendered in that window.
	/// (Don't forget to add this as child somewhere in the hierarchy.)
	/// </summary>
	public class Camera : GameObject {
		public Window RenderTarget {
			get {
				return _renderTarget;
			}
		}
		Window _renderTarget;

		/// <summary>
		/// Creates a camera game object and a sub window to render to.
		/// Add this camera as child to the object you want to follow, or 
		/// update its coordinates directly in an update method.
		/// The scale of the camera determines the "zoom factor" (High scale = zoom out)
		/// </summary>
		/// <param name="windowX">Left x coordinate of the render window.</param>
		/// <param name="windowY">Top y coordinate of the render window.</param>
		/// <param name="windowWidth">Width of the render window.</param>
		/// <param name="windowHeight">Height of the render window.</param>
		public Camera(int windowX, int windowY, int windowWidth, int windowHeight) {
			_renderTarget = new Window (windowX, windowY, windowWidth, windowHeight, this);
			game.OnAfterRender += _renderTarget.RenderWindow;
		}

		/// <summary>
		/// Returns whether a screen point (such as received from e.g. Input.mouseX/Y) is in the camera's window
		/// </summary>
		public bool ScreenPointInWindow(int screenX, int screenY) {
			return
				screenX >= _renderTarget.windowX &&
				screenX <= _renderTarget.windowX + _renderTarget.width &&
				screenY >= _renderTarget.windowY &&
				screenY <= _renderTarget.windowY + _renderTarget.height;
		}

		/// <summary>
		/// Translates a point from camera space to global space, taking the camera transform and window position into account.
		/// The input should be a point in screen space (coordinates between 0 and game.width/height), 
		/// that is covered by the camera window (use ScreenPointInWindow to check).
		/// You can combine this for instance with HitTestPoint and Input.mouseX/Y to check whether the
		/// mouse hits a sprite that is shown in the camera's window.
		/// </summary>
		/// <param name="screenX">The x coordinate of a point in screen space (like Input.mouseX) </param>
		/// <param name="screenY">The y coordinate of a point in screen space (like Input.mouseY) </param>
		/// <returns>Global space coordinates (to be used e.g. with HitTestPoint) </returns>
		public Vector2 ScreenPointToGlobal(int screenX, int screenY) {
			float camX = screenX - _renderTarget.centerX;
			float camY = screenY - _renderTarget.centerY;
			return TransformPoint(camX, camY);
		}

		protected override void OnDestroy() {
			game.OnAfterRender -= _renderTarget.RenderWindow;
		}
	}
}
