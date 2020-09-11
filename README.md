# Countdown Solver in F#

This F# program finds the best solution to a
[Countdown Numbers Game](https://en.wikipedia.org/wiki/Countdown_\(game_show\)#Numbers_round) problem.

## Problem

- 6 numbers are chosen randomly from two sets:
  - up to 4 from the large numbers set `[25,50,75,100]`
  - the remainder from the small numbers set `[1,1,2,2,3,3,4,4,5,5,6,6,7,7,8,8,9,9,10,10]`
- A random 3 digit number from `100-999` is selected
- The 6 numbers should be combined using +,-,*,/ to get as close to the random 3 digit number as possible, ideally exactly
- At no point should the numbers become negative or involve fractions

## Example

Two large numbers `[50;75]` and four small numbers `[2,4,1,4]` are chosen.  
The target number is `178`.  
One solution would be:
- `50 * 2 = 100`
- `100 + 75 = 175`
- `175 + 4 = 179`
- `179 - 1 = 178`

