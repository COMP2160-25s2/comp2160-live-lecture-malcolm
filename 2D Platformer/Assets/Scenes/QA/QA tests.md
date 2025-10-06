# QA Tests

## Collider choice

### QA1a

**Requirement**: The player avatar should be able to stand on the edge of a platform without falling, with only a small amount of its bounding box touching the edge.

**Test scene**: `QA1a - Collider`
 
**Scene setup**: The player avatar is positioned so that only a very small amount of its bounding box is on the platform. The pivot point is in empty space.
 
**Instructions & Observations**: 

* Run the scene without any input.
* Observe the movement of the avatar.
 
**Expectations**:
* The avatar should sit on the edge of the platform without falling.
 
**Conducted**: 

| Date | Tester Name | Commit ID | IDs of bugs encountered | 
| ---- | ----------- | -------- | ----------------------- |
| Oct 6 | Malcolm Ryan | *68c9f76* | n.a. |
