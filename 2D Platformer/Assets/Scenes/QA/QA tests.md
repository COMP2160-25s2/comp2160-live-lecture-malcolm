# QA Tests

## Collider choice

### QA1a - Standing on platform edge

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
| Oct 6 | Malcolm Ryan | efdd43a | n.a. |

### QA1b - Falling down 1x1 hole

**Requirement**: The player avatar should be able to fall down a 1-tile hole between platforms.

**Test scene**: `QA1b - Collider`
 
**Scene setup**: The player avatar is positioned in empty space above a 1 tile hole.
 
**Instructions & Observations**: 

* Run the scene without any input.
* Observe the movement of the avatar.
 
**Expectations**:
* The avatar should fall down the hole without interacting with the platforms on either side.
 
**Conducted**: 

| Date | Tester Name | Commit ID | IDs of bugs encountered | 
| ---- | ----------- | -------- | ----------------------- |
| Oct 6 | Malcolm Ryan | efdd43a | n.a. |

### QA1c - Falling down 1x1 hole touching side

**Requirement**: The player avatar should be able to fall down a 1-tile hole between platforms, even if it is touching the edge of the hole.

**Test scene**: `QA1c - Collider`
 
**Scene setup**: The player avatar is positioned in empty space above a 1 tile hole, so that the left edge coincides exactly with the right edge of the hole.
 
**Instructions & Observations**: 

* Run the scene without any input.
* Observe the movement of the avatar.
 
**Expectations**:
* The avatar should fall down the hole without interacting with the platforms on either side.
 
**Conducted**: 

| Date | Tester Name | Commit ID | IDs of bugs encountered | 
| ---- | ----------- | -------- | ----------------------- |
| Oct 6 | Malcolm Ryan | efdd43a | 001 |


# Bug Report

For each bug you find during testing, complete the following template:

## Bug 001

**Severity**: 4 (Design query)
 
**Date encountered**: 6 Oct 2025
 
**Encountered in build**: efdd43a
 
**Affected Features**: Avatar Collider
 
**Description**: If the avatar falls down a hole with the left edge of the collider exactly aligned with the right edge of the hole, it will move slightly to the right. Is this intended behaviour?
 
**QA Scene**: `QA1c - Collider`
 
**Instructions to reproduce**:
 
* Run the scene without any input.
* Observe the movement of the avatar.

**Expected correct result**:

* The avatar should fall down the hole without interacting with the platforms on either side.

**Actual incorrect result**:

* The avatar falls down the hole but moves slightly to the right.

**Build where resolved (if any)** (Git Commit ID): Unresolved
