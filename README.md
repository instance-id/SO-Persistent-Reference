# Scene Object / ScriptableObject persistent references
This project provides an example of taking scene objects in the hierarchy and creating a persistent reference within ScriptableObjects through code, or by dragging them into the interface. The references will be maintained through Domain Reloads, Entering PlayMode, and closing Unity completely.

---
![](https://i.imgur.com/W0nlLqr.png)

# Example Instructions 
An example scene is located in Asset/instance.id/SOReference/Example/BindingExample.unity

1. To use the example, click on the Reference Manager. In the ObjectHandler component select one or more "TypeData" items from the list, or manually drag GameObjects from the scene into the "Object Reference List".
##### (The TypeData component is used as a tag to demonstrate locating and selecting different component types via code and creating a persistent reference to them within ScriptableObjects. There is a ScriptableObject for each "TypeData" class that uses dropdown to select an identifier that matches it to one of the TypeData classes in order to act as different storage containers for the references based on the Scene Objects assigned type.  
![](https://i.imgur.com/0RTvSKW.png)

2. Press the "Bind Object" button  

![](https://i.imgur.com/JZc5Rpb.png)

3. The Scene Objects should now be seen in the above "Reference Manager" Component under the "Referenced Scene Objects" Foldout.  

![](https://i.imgur.com/BVI8fGy.png)



4. You can also, under the  "Referenced ScriptableObjects foldout, select the ScriptableObject which is of the same TypeData that you selected earlier and then see the Scene Objects added to the ScriptableObject. You should now be able to save the scene, enter playmode, close Unity, etc, and those references will persist. 

![](https://i.imgur.com/ufFVi2G.png)  

![](https://i.imgur.com/p1fp7t0.png)


---
![alt text](https://i.imgur.com/cg5ow2M.png "instance.id")

