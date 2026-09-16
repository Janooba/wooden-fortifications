# Latest Changelog

## v2.1.0
    **New**
    - Redid stake damage calculation and dealing to be more consistent.
    - Added archer's stakes in bismuthbronze, tinbronze, blackbronze, iron, meteoric iron, and steel
        - Damage and durability scales to match each metal's vanilla spear stats.
        - Iron, meteoric iron, and steel archer's stakes are indestructible.
    - Added a config option to disable stake durability entirely.
    - Merged the copper archer's stake into the new metal stake line; existing copper stakes migrate automatically.

    **Fixes**
    - Fixed archer's stakes not dealing damage when hit from the front.
    - Fixed missing handle/string textures on the copper archer's stake.
    - Fixed a potential crash when stacking palisade blocks with a missing variant.

## v2.0.11
    - Fix broken call to changed API causing crash

## v2.0.9
    - Fix for crashing when trying to remove posts with other posts above them
    - Fix for finicky stability often causing entire walls to collapse

**New Known Issues**
    - Stability will allow floating structures if they are "supporting eachother". Unsure if this is worth fixing.

## v2.0.8
    - Version bump

## v2.0.7
    - Allow palisade blocks that are connected to something solid on either side or its front (solid) face to be considered "supported"
    - Fix issue where stacking a palisade block can destroy the block above completely

## v2.0.6
    - Fixed recipes up to use wildcard domains for mod support

## v2.0.5
    - Fixed sounds on certain events that were missing them (place and break)
    - Corners now properly drop their blocks when broken
    - Platforms can now be attached to corners
    - Fixed some odd shading of corners under certain configurations