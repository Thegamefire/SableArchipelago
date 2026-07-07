from dataclasses import dataclass
from Options import PerGameCommonOptions, Choice, DeathLink, Toggle


class Goal(Choice):
    """Choose the end goal.
    gliding: Choose your mask and complete the gliding.
    all_masks: Collect every mask in the game.
    """
    display_name = "Goal"
    option_gliding = 0
    option_all_masks = 1
    default = 1

class RandomizeMasks(Choice):
    """Whether to randomize masks
    off: All masks are at their starting locations.
    shuffle: All masks are at a location of another mask.
    on: All masks are in the itempool.
    """
    display_name = "Randomize Masks"
    option_off = 0
    option_shuffle = 1
    option_on = 2
    default = 1

@dataclass
class SableOptions(PerGameCommonOptions):
    goal: Goal
    death_link: DeathLink
    randomize_masks: RandomizeMasks