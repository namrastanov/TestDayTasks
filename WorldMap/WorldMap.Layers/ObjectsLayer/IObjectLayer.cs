using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldMap.Domain;

namespace WorldMap.Layers.ObjectsLayer
{
    public interface IObjectLayer
    {
        /// <summary>
        /// Gets an object at the specified coordinates
        /// </summary>
        /// <param name="x">The x-coordinate</param>
        /// <param name="y">The y-coordinate</param>
        /// <returns>The game object at the coordinates or null if not found</returns>
        Task<GameObject?> GetByCoordinatesAsync(int x, int y);

        /// <summary>
        /// Checks if a game object is inside a specified rectangular area
        /// </summary>
        /// <param name="gameObject">The game object to check</param>
        /// <param name="topLeftX">The x-coordinate of the top-left corner of the area</param>
        /// <param name="topLeftY">The y-coordinate of the top-left corner of the area</param>
        /// <param name="width">The width of the area</param>
        /// <param name="height">The height of the area</param>
        /// <returns>True if the object is inside the area, false otherwise</returns>
        bool CheckIfInsideArea(GameObject gameObject, int topLeftX, int topLeftY, int width, int height);

        /// <summary>
        /// Gets all objects within a specified rectangular area
        /// </summary>
        /// <param name="topLeftX">The x-coordinate of the top-left corner of the area</param>
        /// <param name="topLeftY">The y-coordinate of the top-left corner of the area</param>
        /// <param name="width">The width of the area</param>
        /// <param name="height">The height of the area</param>
        /// <returns>A collection of game objects within the specified area</returns>
        Task<IEnumerable<GameObject>> GetByAreaAsync(int topLeftX, int topLeftY, int width, int height);

        /// <summary>
        /// Adds a new object to the layer
        /// </summary>
        /// <param name="gameObject">The object to add</param>
        Task AddObjectAsync(GameObject gameObject);

        /// <summary>
        /// Updates an existing object in the layer
        /// </summary>
        /// <param name="gameObject">The object to update</param>
        Task UpdateObjectAsync(GameObject gameObject);

        /// <summary>
        /// Removes an object from the layer by its ID
        /// </summary>
        /// <param name="id">The ID of the object to remove</param>
        Task RemoveObjectAsync(string id);

        /// <summary>
        /// Gets an object by its ID
        /// </summary>
        /// <param name="id">The ID of the object</param>
        /// <returns>The game object or null if not found</returns>
        Task<GameObject?> GetObjectByIdAsync(string id);

        /// <summary>
        /// Subscribes to object change events
        /// </summary>
        /// <param name="handler">The event handler for object changes</param>
        void Subscribe(IObjectChangeHandler handler);

        /// <summary>
        /// Unsubscribes from object change events
        /// </summary>
        /// <param name="handler">The event handler to unsubscribe</param>
        void Unsubscribe(IObjectChangeHandler handler);
    }
}

