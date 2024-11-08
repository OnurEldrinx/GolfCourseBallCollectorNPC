# Game Mechanics Documentation
## 1. Map Design
- Tool Used: Unity Terrain Tools
- Description: The map is crafted using Unity's Terrain Tools, offering a realistic environment for gameplay.
## 2. Character Controller
- System: Unity NavMesh
- Description: The character controller leverages Unity's NavMesh system, enabling smooth and intelligent navigation for NPCs on the terrain.
## 3. Golf Ball Design
- Structure: Scriptable Object-Based Design
- Functionality:
Golf balls are implemented using Unity's scriptable objects, allowing for standardized data containers for each instance.
- Attributes Stored: Each golf ball instance has specific information like point value, priority, material, etc.
## 4. Points System
- Purpose: Rewards NPCs based on collected golf balls' point values.
- Mechanics: Points are awarded by considering the point value and priority of the collected golf ball, ensuring priority-based rewards.
## 5. Health System
- Type: Time-Dependent System
- Mechanics:
The health of the NPC decreases by one every second.
## 6. Decision-Making Algorithm
- Approach: Basic Behavior Tree System
- Root Node: Priority Selector Node
- Child Nodes: Two sequence nodes and one leaf node
- Node Functions:
- Selector Nodes: Act as logical OR gates, allowing flexible decision-making.
- Sequence Nodes: Function as logical AND gates, ensuring multiple conditions are met for specific behaviors.
- Fallback Mechanism: If the NPC’s health falls below a threshold, the behavior tree triggers a fallback. This prompts the NPC to update its decision-making process based on the new health state.
![Tree](https://github.com/user-attachments/assets/6952e479-4308-44f0-8096-4fb82845feab)
## Decision Fallback Moment Below 25 Health:

![DecisionFallback](https://github.com/user-attachments/assets/7dae8857-6899-4df0-9240-974f6f33f0c2)

## Assumptions:
- The NPC can carry only one ball at a time.

## Video Presentation:
[![Watch the video](https://img.youtube.com/vi/HyAAwE7HtVc/maxresdefault.jpg)](https://youtu.be/HyAAwE7HtVc)
