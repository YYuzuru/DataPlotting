
## Functionality:
- Plot functions in 2D and 3D.
- Allows customization of plots like colour, line strength, point density, and plot type
- Includes addons for legends, plot customization, and plot picking
- Provides access to function extrema within the visible space
- Create your own functions simply by implementing interfaces
## Limitations:
- Only supports differentiable and continuous functions.
- Points are limited by natural limit of [maskableGraphic](https://docs.unity3d.com/2019.1/Documentation/ScriptReference/UI.MaskableGraphic.html)
- mostly single-threaded

# Setup
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

## Addons
### Legend
This addon will display all active #IPlot of the referenced #IGraph.

Drag and drop `Legends.Parent` prefab into the scene. If you can not see this in playmode, a canvas-component is most likely missing in one of the parent-gameobjects. To link the legend with a graph, you have to refence the #IGraph component in the legend-script. This is located in ``Legends.Parent>Legends`` and the gameobject `Legends` should contain a component named `Legends Controller Prefab`. Here, link #IGraph with ``graph GO``.
`Element Prefab` is the blueprint for every plot, which will be displayed on the Legend.
### Plot Customizer
This addon is a simple example of what is possible to customize. It will only show visible #IPlot of the graph and  allows for changing of the function name and plot type. In addition it will display the current colour-scheme of the plot. 
It is strongly recommended to customize one for their own usage.

Drag and Drop `PlotCustomizer` onto the scene. It does not require a parent with Canvas. 
Reference #IGraph by assigning it in the inspector at ``graph GO``.
### Plotpicker
This addon will take a ray of your input device and cast it on the #IGraph. Every active #IPLot will return a value, which will be displayed either floating or on a table.

Drag and Drop `PlotPicker.Parent` into the scene. It is strongly recommended to make this a child of your #IGraph. It will try to find the #IGraph on it own. The prefab should be at the origin point of the #IGraph. To give the Plotpicker a desired point, call from `PointerOnGraph.Invoke(Ray castPoint)`
with `castPoint` being the ray of your viewport in combination with the location of your inputDevice.

## Extrema
Each function has a function, which will return min and max for the visible space implemented at #Function level.


# Code
## #Graph

### #IGraph
Represents a graph with an associated axis.
#### Properties
- `IAxis axis`: Gets the axis associated with this graph.
- `RectTransform rectTransform`: Gets the rect transform associated with this graph.
- `GameObject gameObject`: Gets the game object associated with this graph.
#### Methods
- `IPlot[] GetActivePlots()`: Gets the active plots associated with this graph. Returns an array of `IPlot` plots.
### #IGraph2D

Represents a 2D graph with an associated axis. Inherits from #IGraph
#### Properties

- `IAxis2D axis`: Gets the 2D axis associated with this graph.
#### Methods

- `IPlot2D AddPlot(IFunction2D function, params Color[] color)`: Adds a 2D plot to this graph using the specified function and colors. Returns the created plot.

### #IGraph3D
Represents a 3D graph with an associated axis. Inherits from #IGraph
#### Properties
- `IAxis3D axis`: Gets the 3D axis associated with this graph.
#### Methods
- `IPlot3D AddPlot(IFunction3D function, params Color[] color)`: Adds a 3D plot to this graph using the specified function and colors. Returns the created plot.



## #Axis
### #IAxis
Represents an axis associated with a graph.
#### Properties
- `graph`: Gets the graph associated with this axis.
- `gameObject`: Gets the game object associated with this axis.
#### Methods
- `Vector3 GetAxisZero()`: Gets the zero-point of the axis in world space.
- `Vector2 GetAxisZero2D()`: Gets the zero-point of the axis in canvas space.
- `Vector2 GetScale()`: Calculates the occupied world space of the target space (e.g axis-canvasSpace) and divides it by the interval-distance of the axis.
- `Vector2[] GetAxisWorldCorners()`: Gets the corners of the axis in world space.
- `Vector2[] GetAxisCorners()`: Gets the corners of the axis in local space.
- `Vector2[] GetAxisOffset()`: Gets the offset of the axis to the parent canvas.
- `float[] GetXAxisIntervall()`: Gets the x-axis interval. It will not auto transform for logarithmic axis from 0 to 10^0 = 1. From these options 0 will be returned.
- `float[] GetYAxisIntervall()`: Gets the y-axis interval. It will not auto transform for logarithmic axis from 1 to 10^1 = 10. From these options 1 will be returned.
- `Vector2 GetStepSize()`: Gets the distance between each tick/step.
- `Vector2 GetAxisSize()`: Gets the area available for plots to be drawn on.
- `Vector3 TranslateToWorld(Vector2 point)`: Transforms the coordinate of a plot into the local-World-position. To get the global position (e.g. for Gizmos.Draw purposes). Apply axis.gameobject.transform.TransformPoint(Vector2 local-coordinate) after using this function.
- `Vector2 TranslateToAxis(Vector3 point)`: Transforms the coordinate of a world-position into axis-coordinate. You might need to transform the global-world-position to a local one using gameobject.transform.InverseTransformPoint(Vector3). (0,0) will be considered the axis-zero

### #IAxis2D
Represents a 2D axis associated with a graph. Inherits from #IAxis.
#### Properties
- `List<IAxis2D> plotList`: Gets the list of 2D plots associated with this axis.
- `IGraph2D graph`: Gets or sets the graph associated with this axis.
- `bool useXLog`: Gets or sets a value indicating whether the x-axis is logarithmic.
- `bool useYLog`: Gets or sets a value indicating whether the y-axis is logarithmic.
#### Methods
- `IPlot2D AddPlot(IFunction2D function)`: Adds a 2D plot to this axis.
### #IAxis3D
Represents a 3D axis associated with a graph. Inherits from #IAxis.
### Properties
- `List<IPlot3D> plotList`: Gets the list of 3D plots associated with this axis.
- `IGraph3D graph`: Gets or sets the graph associated with this axis.
### Methods
- `IPlot3D AddPlot(IFunction3D function)`: Adds a 3D plot to this axis.

### Customization
Axis can be customized by selecting the `Axis` gameobject in the hierarchy and use the Inspector to set its values.
`Left Space`, `Right Space`, `Top Space`, `Bottom Space`: Determine the space between the canvas edges and the start/end position of an axis.
`Line Strength`: Sets the thickness of the axis. Note that both axis, so the axis pair, always has the same thickness.
`Single Sector Axis`: If toggled, the two axis will no longer intersect at their zeros (if value range contains them) but always intersect at the lowest values of each axis.
`Redreaw Axis`: Forces the plot to redraw the axis on screen and show made changes.

### Ticks
Note that width and height of the ticks always count for axis in x-orientation, means horizontally. So the tick-width is always along the axis direction while the tick-height is always perpendicular to it.
`Tick Width`: Sets the width of ticks.
`Tick Height`: Sets the hight of ticks.
`Tick Position`: Takes the Int inputs -1, 0, 1 and sets the position of the ticks on the axis. -1 makes them go inwards in the plot, 1 makes them go outwards, 0 centers the ticks on the axis and makes them halfway in and out.
`Redraw Tick Lables`: Forces the plot to redraw the ticks and show made changes. It also repositions the ticks according to to axis. 

## #Plot
### #IPlot

Represents a plot. Inherits from `IColorable`.
#### Properties
- **IAxis axis**: Gets the axis of the plot.
- **GameObject gameObject**: Gets the game object of the plot.
- **IFunction function**: Gets the function of the plot.
#### Methods
- **void Refresh()**: Refreshes the plot.
- **void SetPlotType(int plotType)**: Sets the type of the plot.
- **int GetPlotType()**: Gets the type of the plot.

### #IPlot2D
Represents a 2D plot. Inherits from #IPlot.

#### Properties
- **ePlot2D plotType**: Gets the type of the 2D plot.
- **IFunction2D function**: Gets the function of the 2D plot.
#### Methods
- **Vector2 GetSignal(float position)**: Gets the signal of the 2D plot at a given position.
- **void SetLineStrength(float strength)**: Sets the line strength of the 2D plot.
- **float GetLineStrength()**: Gets the line strength of the 2D plot.
- **void SetPointDensity(float density)**: Sets the point density of the 2D plot. A maximum of ~5000 points should is recommended. Higher = more space between each point. 
- **float GetPointDensity()**: Gets the point density of the 2D plot.

### #IPlot3D
Represents a 3D plot. Inherits from #IPlot.
#### Properties
- **ePlot3D plotType**: Gets the type of the 3D plot.
- **IFunction3D function**: Gets the function of the 3D plot.
#### Methods
- **void SetIntervall(Vector2 start, Vector2 end, Vector2 stepSize)**: Sets the interval of the 3D plot.
- **Vector3 GetSignal(Vector2 position)**: Gets the signal of the 3D plot at a given position.

## #Function
### #IFunction

Represents a mathematical function.

### Properties

- `string name`: Gets or sets the name of the function.

### #IFunction2D
Represents a 2D mathematical function. Inherits from `IFunction`.
There are two types of `IFunction2D`: One takes a function `Func<float,float>` and is created which is equivalent with `f(x) = y` with `x,y` being float named: `Function` and one simply consisting of a list of points `LiveFeed`.

To create a `IFunction2D`, use the constructor of `Function(Func<float,float>,string name)` or if you want to create one with only a list of points use `LiveFeed(string name)` and add points yourself.

To update `LiveFeed` with additional points, use `Update(List<Vector2> points)`.  Another way, which might cause issues, use `List<Vector2> datapoints`.  Remember that this plugin officially only supports  differentiable and continuous function.
#### Properties

- `Vector2[] max`: Gets the maximum values of the function.
- `Vector2? maxGlobal`: Gets the global maximum values of the function.
- `Vector2[] min`: Gets the minimum values of the function.
- `Vector2? minGlobal`: Gets the global minimum values of the function.

#### Methods
- `List<Vector2> GetIntervall(float start, float end, float step, bool useLog)`: Gets the interval of the function.
- `Vector2 GetSignal(float position)`: Gets the signal of the function at a given position.

### #IFunction3D
Represents a 3D mathematical function. Inherits from `IFunction`.
To create a `IFunction3D`, use the constructor of `FunctionHeatMap(DataPoint[,] function,string name)` with `Datapoint(Vector2 point,float signal)`.

To insert a list of datapoints into this function from csv-file, The use of `DataGenerator.Compile(CSV path, params char[] splitter)` is recommended. the `CSV` struct takes three csv files. One for x, a second one for y and a third for the signal.

#### Properties
- `Vector3[] max`: Gets the maximum values of the function.
- `Vector3? maxGlobal`: Gets the global maximum values of the function.
- `Vector3[] min`: Gets the minimum values of the function.
- `Vector3? minGlobal`: Gets the global minimum values of the function.
#### Methods
- `DataPoint[,] GetIntervall()`: Gets the interval of the function.
- `Vector3 GetSignal(Vector2 position)`: Gets the signal of the function at a given position.