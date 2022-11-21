namespace Heroes.ECS;

public class ComponentSystem {
    private Hero[]? components;

    ///<summary>Get all components on the hero/summary>
    public Hero[]? GetComponents() {
        return components;
    }

    ///<summary>Get the first component by type.</summary>
    ///<param name="type">The type of the component.</param>
    ///<returns>The component (T of Hero).</returns>
    public T? GetComponent<T>() where T : Hero {
        // Check if the components are null
        if (components == null) return null;

        // Loop through the components
        foreach (Hero component in components) {
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
    ///<returns>An array of components (T of Hero).</returns>
    public T[]? GetComponents<T>() where T : Hero {
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
    public T RegisterComponent<T>() where T : Hero, new() {
        // Create a new component
        T component = new T();

        // Check if components is null
        if (components == null) {
            // Set components to a new array
            components = new Hero[] { component };
        } else {
            // Create a new array
            Hero[] newComponents = new Hero[components.Length + 1];

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

    ///<summary>Get all components.</summary>
    ///<returns>An array of all components.</returns>
    public Hero[]? GetAllComponents() {
        // Return the components
        return components;
    }

    ///<summary>Delete a component of type T.</summary>
    ///<param name="type">The type of component to delete.</param>
    public void DeleteComponent<T>() where T : Hero {
        // Check if the components are null
        if (components == null) return;

        // Create a copy of the components
        Hero[] copy = new Hero[components.Length];

        // Create a list of components
        List<Hero> list = new List<Hero>();

        // Loop through the components
        bool deleted = false;
        foreach (var component in components) {
            // Check if the component is null
            if (component == null) continue;

            // Check if the component is of type T
            if (!deleted && component is T) {
                // Set deleted to true
                deleted = true;

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
    public void DeleteComponents() {
        // Set components to null
        components = null;
    }
}