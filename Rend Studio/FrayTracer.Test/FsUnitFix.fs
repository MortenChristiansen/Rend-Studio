module FsUnitFix

open FrayTracer
open Mathematics
open Geometry
open NHamcrest
open NHamcrest.Core
open System

let rec equalsWithToleranceStandard (t:obj) (x:obj) (a:obj) =
    let actualParsed, actual = Double.TryParse(string a, System.Globalization.NumberStyles.Any, new System.Globalization.CultureInfo("en-US"))
    let expectedParsed, expect = Double.TryParse(string x, System.Globalization.NumberStyles.Any, new System.Globalization.CultureInfo("en-US"))
    let toleranceParsed, tol = Double.TryParse(string t, System.Globalization.NumberStyles.Any, new System.Globalization.CultureInfo("en-US"))
    if actualParsed && expectedParsed && toleranceParsed then
        abs(actual - expect) <= tol
        else false

let vectorsEqualWithTolerance t x a =
    equalsWithToleranceStandard t x.X a.X && equalsWithToleranceStandard t x.Y a.Y && equalsWithToleranceStandard t x.Z a.Z

let raysEqualWithTolerance t x a =
    vectorsEqualWithTolerance t x.Origin a.Origin && vectorsEqualWithTolerance t x.Direction a.Direction

let colorsEqualWithTolerance t x a =
    equalsWithToleranceStandard t x.R a.R && equalsWithToleranceStandard t x.G a.G && equalsWithToleranceStandard t x.B a.B

let rec equalsWithTolerance (t:obj) (x:obj) (a:obj) =
    match x, a with
    | (:? Vector as vX), (:? Vector as vA) -> vectorsEqualWithTolerance t vX vA
    | (:? Ray as rX), (:? Ray as rA) -> raysEqualWithTolerance t rX rA
    | (:? Color as cX), (:? Color as cA) -> colorsEqualWithTolerance t cX cA
    | _ -> equalsWithToleranceStandard t x a

let equalWithin (t:obj) (x:obj) = CustomMatcher<obj>(sprintf "%s with a tolerance of %s" (x.ToString()) (t.ToString()), fun a -> equalsWithTolerance t x a)