# Changelog

Last tested with RJW 4.9.6.1

## v0.11

- Updated for RJW 5.3.0.5.
- SexAbility references removed or replaced with SexFrequency references.
- Rebalanced serum prices.

## v0.10.1

- Updated for RJW 4.9.6.1.
- By amevarashi:
  - Fixes issues in determining penetrator/penetratee.
  - Prevent double-counting of stretching or cumflation.

## v0.10

- Updated for RJW 4.9.4.2.

## v0.9.1

- Hotfix to fix a nullable pawn problem when checking for deadness.

## v0.9

- Updated for RJW 4.8.1.1.
- Cumflation calculations now occur whenever an orgasm occurs (TransferNutrition is called).
- Cumflation now satisfies thirst.
- Pawns with the "Likes Cumflation" trait no longer vomit, but instead leak.
- Stretching and cumflation mechanics now use new RJW penetration determiner, so results from sex are probably more consistent.
- Added a record for cumstuffing and cumflation.

## v0.8.1

- Hotfix to fix a problem with cum vomit.

## v0.8.0

- Updated for Rimworld 1.3 and RJW 4.7.1.1.
- Added cum vomit.
- Fixed a bug where oral cumstuffing did not work.
- Increased threshold for prolapse damage.

## v0.7.4

- Continued work on the recipe patching, with help from Abraxas.
- Added Russian localisation, courtesy of Natsu_Zirok.
- Added a null check for GetAdjustedHediffDimensions.

## v0.7.3

- Added prolapse surgery to a recipe patch so that the surgeries are usable.
- Changed Harmony integration from HarmonyPrefix to HarmonyPostfix to prevent Pawns from getting downed mid-sex.
- Removed consciousness penalties from cumflation to prevent death from consciousness loss.
- Fixed a bug where cumflation would not be persistent.
- Added a null check in the GetAdjustedHediffDimensions function.

## v0.7.2

- Made vomit occur immediately after sex instead of interrupting it.

## v0.7.1

- Fixed a bug where the penetrator pawn was being considered for serum/body effects in orifice calculations.
- Minor code changes.

## v0.7.0

- Overhauled stretching calculation to take into account multiple penetrators and extended RJW values (i.e. length and girth).
- Difficulty of extreme stretching has increased; extremely overstretched orifices can take up to 4 days to restore.
- Prolapse and damage are no longer chance based but size-based.
- Damage caused to stretched orifices has increased in severity, meaning that a number of stretching penetrations can destroy an orifice.
- New Hediff added: "CervixPunch".
- Added option to violate thermodynamics when transferring nutrition (i.e. pawns can transfer nutrition even if they themselves have none left).
- When cumflation is enabled, too much anal cumflation will overflow into the stomach, and too much overflow from the stomach will induce vomiting.
- Settings have been cleaned up; old settings file will error out.

## v0.6.1

- Added option to transfer nutrition upon orally cumming (cumflation amount is unaffected).
- Added "Likes Cumflation trait" serum (for adding and removing serum through gameplay).

## v0.6

- Slightly adjusted text and effects of cumflation in general.
- Added oral cumflation.
- Added overinflation effects (just mood, no health).
- Added "Likes Cumflation" trait.

## v0.5f - Ed86

- Raised serum prices and production costs
- Changed research tree, most research are now spacer tech
- Moved gender serums to own research
- Slightly changed some descriptions
- Removed double effect from temp serums
- Changed serum effect from sexability to sexdrive

## v0.5e - Ed86

- Updated for RJW 4.2.0.

## v0.5d - Ed86

- Changed stretching from random rolls to size comparison.
- Permanent stretching = pp 150% > orifice
- Temp stretching = pp > orifice
- Changed cuminflation to use real ejaculation value
- Fixed genital growing serums

## v0.5c - Skömer

- Growth/reduction rework: serums add/remove 10% size on a scale where "huge" is 90%, so you may need multiple serums to go a rjw size up (from average to big for example). Stretching  uses the same system.
- Inflation calculations have been reworked. Inflation multiplier slider can be set to up to 20x.
- You'll need rjw race support mod for the futa/genderbend/reverter to work as intended if you use alien races.

## Other

Updated to 1.1 by Ed86
Updated to 1.1 by Linkolas
Based on mod version: 0.4b by LustLicentia
Preview image provided by the lovely yuriob262.
