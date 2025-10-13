using WorldMap.Layers.ObjectsLayer.Base;

namespace WorldMap.Layers.ObjectsLayer
{
    public interface IObjectLayer
    {
        /// <summary>
        /// Gets all objects within the specified area
        /// </summary>
        /// <param name="x1">Start X coordinate</param>
        /// <param name="y1">Start Y coordinate</param>
        /// <param name="x2">End X coordinate</param>
        /// <param name="y2">End Y coordinate</param>
        /// <returns>Collection of game objects in the area</returns>
        Task<IReadOnlyCollection<GameObject>> GetObjectsInAreaAsync(int x1, int y1, int x2, int y2);

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

