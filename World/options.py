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

@dataclass
class SableOptions(PerGameCommonOptions):
    goal: Goal
    death_link: DeathLink