using WorldMap.Domain;

namespace WorldMap.Application
{
    public interface IObjectChangeHandler
    {
        /// <summary>
        /// Called when a new object is added to the layer
        /// </summary>
        /// <param name="gameObject">The added object</param>
        Task OnObjectAddedAsync(GameObject gameObject);

        /// <summary>
        /// Called when an object is updated in the layer
        /// </summary>
        /// <param name="gameObject">The updated object</param>
        Task OnObjectUpdatedAsync(GameObject gameObject);

        /// <summary>
        /// Called when an object is removed from the layer
        /// </summary>
        /// <param name="id">The ID of the removed object</param>
        Task OnObjectRemovedAsync(string id);
    }
}

