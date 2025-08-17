using System.Collections.Generic;

namespace Heroes.Attachables;

public class HeroAttachableSystem : Common {

    public Hero? Parent;
    private Attachable[]? components;

    ///<summary>Get all components on the hero</summary>
    public Attachable[]? GetAttachables() {
        return components;
    }

    ///<summary>Get the first component by type.</summary>
    ///<param name="type">The type of the component.</param>
    ///<returns>The component (T of Attachable).</returns>
    public T? GetAttachable<T>() where T : Attachable {
        // Check if the components are null
        if (components == null) return null;

        // Loop through the components
        foreach (var component in components) {
            // Check if the component is null
            if (component == null) continue;

            // Check if the component is of type T
            if (component is T) return (T)component;
        }

        // Return null
        return null;
    }

    ///<summary>Get an array components by type.</summary>
    ///<param name="type">The type of component to get.</param>
    ///<returns>An array of components (T of Attachable).</returns>
    public T[]? GetAttachables<T>() where T : Attachable {
        // Check if the components are null
        if (components == null) return null;

        // Create a list of components
        List<T> list = new List<T>();

        // Loop through the components
        foreach (var component in components) {
            // Check if the component is null
            if (component == null) continue;

            // Check if the component is of type T
            if (component is T) list.Add((T)component);
        }

        // Return the list
        return list.ToArray();
    }

    ///<summary>Register a component.</summary>
    ///<param name="component">The component to register.</param>
    public T Attach<T>() where T : Attachable, new() {
        // Create a new component of type T and provide the parent
        T component = new T();
        component.Parent = Parent;

        // Check if components is null
        if (components == null) {
            // Set components to a new array
            components = new Attachable[] { component };
        } else {
            // Create a new array
            Attachable[] newComponents = new Attachable[components.Length + 1];

            // Loop through all components
            for (int i = 0; i < components.Length; i++) {
                // Set the new array to the old array
                newComponents[i] = components[i];
            }

            // Set the last index to the new component
            newComponents[newComponents.Length - 1] = component;

            // Set components to the new array
            components = newComponents;
        }

        // Return the component
        return component;
    }

    ///<summary>Delete a component.</summary>
    ///<param name="component">The component to delete.</param>
    public void DeleteAttachable(Attachable component) {
        // Check if the components are null
        if (components == null) return;

        // Run the delete method
        component.Destroy();

        // Create a copy of the components
        Attachable[] copy = new Attachable[components.Length];

        // Create a list of components
        List<Attachable> list = new List<Attachable>();

        // Loop through the components
        bool deleted = false;
        foreach (var c in components) {
            // Check if the component is null
            if (c == null) continue;

            // Check if the component is the component
            if (!deleted && c == component) {
                // Set deleted to true
                deleted = true;

                // Continue
                continue;
            }

            // Add the component to the list
            list.Add(c);
        }
        
        // Set components to the copy
        components = list.ToArray();
    }

    ///<summary>Delete a component of type T.</summary>
    ///<param name="type">The type of component to delete.</param>
    public void DeleteAttachable<T>() where T : Attachable {
        // Check if the components are null
        if (components == null) return;

        // Create a copy of the components
        Attachable[] copy = new Attachable[components.Length];

        // Create a list of components
        List<Attachable> list = new List<Attachable>();

        // Loop through the components
        bool deleted = false;
        foreach (var component in components) {
            // Check if the component is null
            if (component == null) continue;

            // Check if the component is of type T
            if (!deleted && component is T) {
                // Set deleted to true
                deleted = true;

                // Run the delete method
                component.Destroy();

                // Continue
                continue;
            }

            // Add the component to the list
            list.Add(component);
        }
        
        // Set components to the copy
        components = list.ToArray();
    }

    ///<summary>Delete all components.</summary>
    public void DeleteAllAttachable() {
        // Check if the components are null
        if (components == null) return;

        // Loop through the components
        foreach (var component in components) {
            // Check if the component is null
            if (component == null) continue;

            // Run the delete method
            component.Destroy();
        }

        // Set components to null
        components = null;
    }
}