# DataPlotting
Primitive Data plotting plugin for unity developed by
Moritz Hetterich and Thomas Nguyen.

### Abstract
Unity DataPlotting is a plugin for the Unity game
engine, allowing developers to seamlessly integrate a data
plot canvas into their projects. With this powerful tool, develop-
ers can visualize 2D and 3D data sets on a 2-dimensional canvas,
presenting them as line graphs, scatter graphs, or heatmaps.
The interactive nature of the plugin enables users to specify
the specific subsection of the data set they wish to display.
Moreover, data visualization can be achieved using either linear
or logarithmic representation. One of the key strengths of Unity
DataPlotting lies in its complete customizability, making it a
perfect fit for any 2D, 3D, or VR project developed within the
Unity game engine.
### Installation and setup
Import the .unitypackage file into your project and add the DataPlot-Prefab to your hierarchy. DO NOT rename the prefab in your hierarchy or any of its children.
## Setting up
Place the provided Prefab into your scene and confirm the presence of both the [EventSystem](https://docs.unity3d.com/2021.3/Documentation/Manual/EventSystem.html) and the [StandaloneInputModule](file:///C:/Program Files/Unity/Hub/Editor/2021.3.11f1/Editor/Data/Documentation/en/Manual/script-StandaloneInputModule.html). The StandaloneInputModule is responsible for handling events for UI elements like buttons and input fields. If these modules are not present, they will be automatically generated when you create a canvas in the scene. However, note that no modules will be created for the prefab itself.

After adding the desired prefab to the scene, connect the camera used for the player to the canvas of the prefab. At this point, you should be able to interact with the graph in the game scene during play mode. 
## Adding plots:
To generate a plot with two values, utilize #IGraph2D; for three values, employ #IGraph3D. Incorporate the plot using `AddPlot(IFunction2D function, params Color[] color)` for #IGraph2D and `AddPlot(IFunction3D function, params Color[] color)` for #IGraph3D. Ensure to retain the returned plot for improved customization accessibility. To create function instances, refer to #Function.

## Removing plots
  
Currently, the sole method for removing a plot is by deleting the corresponding game object. #IGraph's `GetActivePlots()` will automatically update following this action.
## Hiding plots
As of now there is no other way of hiding a plot beside enabling/disabling the gameobject with `setActive(boolean)`. #IGraph `GetActivePlots()` will update automatically.
## Customize plots:
To customize the #IPlot game object, you can cast it to #IPlot2D and #IPlot3D. However, it's essential to be cautious as there are currently no safeguards in place to prevent the use of incorrect or performance-hindering values. Exercise care when applying customizations to ensure optimal performance and accurate representation.


For more information visit the documentation file in Documentation.