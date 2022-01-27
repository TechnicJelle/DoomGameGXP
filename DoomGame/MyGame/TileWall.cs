using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GXPEngine.Core;

namespace GXPEngine.MyGame
{
	public class TileWall : Tile
	{
		private readonly WallSide[] sides;

		public Sprite minimapTexture { get; }

		public TileWall(int col, int row, string filename)
		{
			sides = new WallSide[4];
			for (int i = 0; i < sides.Length; i++)
			{
				sides[i] = new WallSide(filename, i, col + 0.5f, row + 0.5f);
			}

			minimapTexture = new Sprite(filename, true, false);
		}

		/// <param name="visibility">If the entire tile is visible or not</param>
		public void SetVisibility(bool visibility)
		{
			foreach (WallSide side in sides)
				side.SetVisibility(visibility);
		}

		public IEnumerable<WallSide> FindVisibleSides()
		{
			return sides.Where(side => side.AlignsWithPlayer());
		}

		/// <summary>
		/// Function to shoot out a ray from one position to another and see if it hits a tile and where.<br/>
		/// DDA is short for Digital Differential Analyzer and it's used as a replacement for the standard way of ray-casting, which is taking tons of tiny little steps.
		/// This is slow, of course, but it is also inaccurate, because there is a chance that that step size can skip over something!
		/// DDA is an algorithm that is both way faster and also perfectly accurate!
		/// </summary>
		/// <param name="rayStart"> Vector2 position in world space of the start of the ray-to-be-cast </param>
		/// <param name="rayDir">NORMALIZED vector of the direction you want the ray-to-be-cast to go in</param>
		/// <param name="maxDistance">Optional argument to prevent the ray from going too far (also in world space)</param>
		/// <returns>
		/// <li>A reference to the TileWall object the ray hit (null if nothing was hit)</li>
		/// <li>A Vector2 with the exact position of the intersection of the ray with the hit TileWall (null if nothing was hit)</li>
		/// <li>A distance float with the distance the ray travelled, so if a TileWall was hit, it's the distance to the intersection point, but if nothing was this, it's around the same as the given maxDistance</li>
		/// </returns>
		[SuppressMessage("ReSharper", "UseDeconstruction")]
		// ReSharper disable once InconsistentNaming
		public static (TileWall tileWall, Vector2 intersection, float dist) DDA(Vector2 rayStart, Vector2 rayDir, float maxDistance = 100.0f)
		{
			//From https://www.youtube.com/watch?v=NbSee-XM7WA
			Vector2 rayUnitStepSize = new Vector2(Mathf.Abs(1.0f / rayDir.x), Mathf.Abs(1.0f / rayDir.y));

			(int x, int y) mapCheck = ((int)rayStart.x, (int)rayStart.y);
			Vector2 rayLength1D = new Vector2();

			(int x, int y) step;

			// Establish Starting Conditions
			if (rayDir.x < 0)
			{
				step.x = -1;
				rayLength1D.x = (rayStart.x - mapCheck.x) * rayUnitStepSize.x;
			}
			else
			{
				step.x = 1;
				rayLength1D.x = (mapCheck.x + 1 - rayStart.x) * rayUnitStepSize.x;
			}

			if (rayDir.y < 0)
			{
				step.y = -1;
				rayLength1D.y = (rayStart.y - mapCheck.y) * rayUnitStepSize.y;
			}
			else
			{
				step.y = 1;
				rayLength1D.y = (mapCheck.y + 1 - rayStart.y) * rayUnitStepSize.y;
			}

			bool tileFound = false;
			TileWall result = null;
			float distance = 0.0f;
			while (!tileFound && distance < maxDistance)
			{
				// Walk along shortest path
				if (rayLength1D.x < rayLength1D.y)
				{
					mapCheck.x += step.x;
					distance = rayLength1D.x;
					rayLength1D.x += rayUnitStepSize.x;
				}
				else
				{
					mapCheck.y += step.y;
					distance = rayLength1D.y;
					rayLength1D.y += rayUnitStepSize.y;
				}

				// Test tile at new test point
				if (!(mapCheck.x >= 0) || !(mapCheck.x < MyGame.currentLevel.tilesColumns) ||
				    !(mapCheck.y >= 0) || !(mapCheck.y < MyGame.currentLevel.tilesRows)) continue;
				Tile target = MyGame.currentLevel.GetTileAtPosition(mapCheck.x, mapCheck.y);
				if (target.GetType() == typeof(Tile)) continue;
				result = (TileWall) target;
				tileFound = true;
			}

			// Calculate intersection location
			Vector2 intersection = null;
			if (tileFound)
			{
				intersection = Vector2.Add(rayStart, rayDir.Copy().Mult(distance));
			}
			return (result, intersection, distance);
		}
	}
}