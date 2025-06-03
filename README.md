# SpookyCore
A Unity2D game framework to facilitate fast prototyping and can be used as a foundation for further development.

## Contents  
- [EntitySystem](#entitysystem): A custom MonoBehaviour system for tight life cycle control and modular game logic.
- [Systems](#systems):
  - BootstrapSystem: Provides a system to initialize data and systems before gameplay.
  - SingletonSystem: Provides a generic MonoBehaviour singleton and a generic pure singleton.
  - PoolSystem: Provides a custom generic ObjectPool and a PoolSystem for Entity.
  - TimerSystem: Provides a centralized system for managing timers.
  - GlobalAssetRegistry: Provides a centralized system for loading prefabs and scriptable objects at runtime.
  - SceneFlowSystem: Provides a system to manage scene flow.
  - And other systems.
- [UISystem](#uisystem): Provides a Model-View-Presenter (MVP) pattern UI system.
- [AI](#ai): Provides generic AI systems for NPCs, currently only has Behaviour AI.
- [Utilities](#utilities): Some extension methods and attributes.

## EntitySystem
At its core, the EntitySystem includes the 2 basic scripts: [Entity](#entity) and [EntityComponent](#entitycomponent)
### `Entity`
- Any tangible game object in the scene can become an Entity by adding the Entity component to it.
- An Entity has a fully managed life cycle with events and states, from after getting from a pool to before returning to a pool. Alternatively, the life cycle can begin at Awake and end at OnDestroy by toggling `Use Unity Awake and Start` in the Entity component.
- All Entities have a list of their components as well as a unique ID.  
![image](https://github.com/user-attachments/assets/0084282c-5411-4b5f-85f4-fd3c24a9fe63)
- The IDs can be configured in a window editor to make the process easier. The editor tool can be found at `SpookyTools/Entity System/EntityID Editor`. The IDs are split into categories, with 100 available ID slots between them. In the below image, there are 3 categories: Player (100), Enemy (200), and Platform (300).  
![image](https://github.com/user-attachments/assets/efb61587-823e-4fb7-8b8e-a41c9d44944a)
- Once you're satisfied with the ID configurations, select `Generate EntityID and EntityType`, which will generate the `EntityID` enum to be selected as IDs for the entities, and the `EntityType` static class that provides extension methods for checking ID types.  
![image](https://github.com/user-attachments/assets/ec8ae6c0-dd2e-4b37-9d6d-ca3f08f9608c)

### EntityComponent
Entity components are modular logic handlers for Entities. The EntityComponent class itself is abstract, but the system comes prepackaged with a few useful EntityComponents. Components can be quickly added in the custom inspector of the Entity component (yes, I just now realized that EntityComponent, Entity Components, and Entity component are confusing).  
![image](https://github.com/user-attachments/assets/917f1354-d20b-4188-91cc-c80e98885fe4)

- EntityVisual: EntityVisual handles the visual aspect of an Entity, mainly the SpriteRenderer. The custom editor provides an easy button for creating the template hierarchy for the Entity game object. The created hierarchy comes with a default square sprite.
![image](https://github.com/user-attachments/assets/305e2ae6-107b-48e6-a3d5-7ccee706d034)
![image](https://github.com/user-attachments/assets/d81d7fd5-864b-4b30-bae0-4d690cca0dc0)
- EntityAnimation:
  - `EntityAnimation` handles all things animation-related for an `Entity`. An `Animator` and `EntityVisual` will be added if the Entity doesn't have them yet.
  - `EntityAnimation` requires an `EntityAnimationConfig` scriptable object and a `RuntimeAnimatorController`. The `EntityAnimationConfig` holds all the Animation Clips for that `Entity`, while the `RuntimeAnimatorController` is there to be assigned to the `Animator` component and make sure you don't forget it.  
![image](https://github.com/user-attachments/assets/e4d30601-95d6-4e94-81bd-9617079c32c3)
  - The `EntityAnimationConfig` can be created through `Create/SpookyCore/Entity System/Animation/Entity Animation Config`. Once created, don't forget to click `Generate EntityAnimState` to generate the EntityAnimState enum. The enum is there to avoid using strings for controlling animations, which are very error-prone. There is also an `Animation Clip Previewer` window editor to preview 2D Sprites Animation Clips because Unity doesn't have one.  
![image](https://github.com/user-attachments/assets/581e0a64-18db-4a63-a80b-8358b580eebd)
  - You can set up the Animator Controller yourself, and don't forget to rebind the target `SpriteRenderer` to the correct hierarchy, or you can use the provided `Animator Controller Builder`, which will build the Animator Controller, rebind the corrent target path, and save it to a folder of your choice. Once assigned, the first frame of the first clip in the config will be chosen as the default sprite for the Entity.  
![image](https://github.com/user-attachments/assets/1694927a-0feb-4011-914b-54278bef9f7e)
- EntityColider: //Adds collider logic and collider hierarchy
- EntityStat: //Adds stat handling logic for the entity
- EntityHealth: //Adds health logic and health bar
- EntityMovement: //Adds movement logic, with a default 4-directional movement, or a simple character controller movement for platformer games.
- EntityInputReceiver: //Allow the Entity to receive inputs, either from the new input system or the using some template inputs from the old input system.
- EntityEnemyDetector: //Detect enemy entities that have an Entity component and an EntityCollider component attached.
- EntityAttack: //Handle attack logic.
- EntityAI: //Allow adding behaviour tree AI for NPC entities.

## Systems
> Not Documented Yet
## UISystem
> Not Documented Yet
## AI
> Not Documented Yet
## Utilities
> Not Documented Yet
