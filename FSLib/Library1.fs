//
// F# library to analyze Chicago crime data
//
// Maribel Jaramillo
// U. of Illinois, Chicago
// CS341, Fall2016
// Homework 5
//
module FSAnalysis

#light

open System
open FSharp.Charting
open FSharp.Charting.ChartTypes
open System.Drawing
open System.Windows.Forms

//
// Parse one line of CSV data:
//
//   Date,IUCR,Arrest,Domestic,Beat,District,Ward,Area,Year
//   09/03/2015 11:57:00 PM,0820,true,false,0835,008,18,66,2015
//   ...
//
// Returns back a tuple with most of the information:
//
//   (date, iucr, arrested, domestic, area, year)
//
// as string*string*bool*bool*int*int.
//
let private ParseOneCrime (line : string) = 
  let elements = line.Split(',')
  let date = elements.[0]
  let iucr = elements.[1]
  let arrested = Convert.ToBoolean(elements.[2])
  let domestic = Convert.ToBoolean(elements.[3])
  let area = Convert.ToInt32(elements.[elements.Length - 2])
  let year = Convert.ToInt32(elements.[elements.Length - 1])
  (date, iucr, arrested, domestic, area, year)


// 
// Parse file of crime data, where the format of each line 
// is discussed above; returns back a list of tuples of the
// form shown above.
//
// NOTE: the "|>" means pipe the data from one function to
// the next.  The code below is equivalent to letting a var
// hold the value and then using that var in the next line:
//
//  let LINES  = System.IO.File.ReadLines(filename)
//  let DATA   = Seq.skip 1 LINES
//  let CRIMES = Seq.map ParseOneCrime DATA
//  Seq.toList CRIMES
//
let private ParseCrimeData filename = 
  System.IO.File.ReadLines(filename)
  |> Seq.skip 1  // skip header row:
  |> Seq.map ParseOneCrime
  |> Seq.toList


//
// Parse one line of IUCR data:
//   IUCR,Primary Description, Secondary Description
//   110, homicide, first degree murder
//
// Returns back a tuple with most of the information:
//
//   (iucr, primary, secondary)
//
// as string*string*string.
//
let private ParseOneIUCR (line : string) = 
  let elements = line.Split(',')
  let iucr = elements.[0]
  let primary = elements.[1]
  let secondary = elements.[2]
  (iucr, primary, secondary)


// 
// Parse file of IUCR, where the format of each line 
// is discussed above; returns back a list of tuples of the
// form shown above.
//
let private ParseIUCR filename = 
  System.IO.File.ReadLines(filename)
  |> Seq.skip 1  // skip header row:
  |> Seq.map ParseOneIUCR
  |> Seq.toList


// Parse one line of Area data:
// Returns back a tuple with the information:
//
//   (code,name)
//
// as int*string.
//
let private ParseOneArea (line : string) = 
  let elements = line.Split(',')
  let code = Convert.ToInt32(elements.[0])
  let name = elements.[1]
  (code, name)

// 
// Parse file of Area, where the format of each line 
// is discussed above; returns back a list of tuples of the
// form shown above.
//
let private ParseArea filename = 
  System.IO.File.ReadLines(filename)
  |> Seq.skip 1  // skip header row:
  |> Seq.map ParseOneArea
  |> Seq.toList


//
// Given a list of crime tuples, returns a count of how many 
// crimes were reported for the given year:
//
let private CrimesThisYear crimes crimeyear = 
  let numCrimes = crimes
                  |> List.filter (fun (_, _, _, _, _, year) -> year = crimeyear)
                  |> List.length 
  numCrimes


//
// CrimesByYear:
//
// Given a CSV file of crime data, analyzes # of crimes by year, 
// returning a chart that can be displayed in a Windows desktop
// app:
//
let CrimesByYear(filename) = 
  //
  // debugging:  print filename, which appears in Visual Studio's Output window
  //
  //printfn "Calling CrimesByYear: %A" filename
  //
  let crimes = ParseCrimeData filename
  //
  let (_, _, _, _, _, minYear) = List.minBy (fun (_, _, _, _, _, year) -> year) crimes
  let (_, _, _, _, _, maxYear) = List.maxBy (fun (_, _, _, _, _, year) -> year) crimes
  //
  let range = [minYear .. maxYear]
  let countsByYear = range
                     |> List.map (fun year -> CrimesThisYear crimes year)
                     |> List.map2 (fun year count -> (year, count)) range
  //
  // debugging: see Visual Studio's Output window (may need to scroll up?)
  //
  //printfn "Counts: %A" counts
  //printfn "Counts by Year: %A" countsByYear
  //
  // plot:
  //
  let myChart = 
    Chart.Line(countsByYear, Name="Total # of Crimes")
  let myChart2 = 
    myChart.WithTitle(filename).WithLegend();
  let myChartControl = 
    new ChartControl(myChart2, Dock=DockStyle.Fill)
  //
  // return back the chart for display:
  //
  myChartControl


//
// Given a list of crime tuples, returns a count of how many 
// arrests were reported for the given year:
//
let private ArrestsThisYear crimes years= 
  let numArrests = crimes
                   |>List.filter (fun (_, _, arrested, _, _, year) -> year = years && arrested = true)
                   |> List.length
  numArrests


//
// Arrests:
//
// Given a CSV file of crime data, analyzes # of arrests by year, 
// returning a chart that can be displayed in a Windows desktop
// app:
//
let ArrestsByYear(filename) = 
  //
  // debugging:  print filename, which appears in Visual Studio's Output window
  //
  //printfn "Calling ArrestsByYear: %A" filename
  //
  let crimes = ParseCrimeData filename
  //
  let (_, _, _, _, _, minYear) = List.minBy (fun (_, _, _, _, _, year) -> year) crimes
  let (_, _, _, _, _, maxYear) = List.maxBy (fun (_, _, _, _, _, year) -> year) crimes
  //
  let range = [minYear .. maxYear]
  let arrestsByYear = range
                      |> List.map (fun year -> ArrestsThisYear crimes year)
                      |> List.map2 (fun year count -> (year, count)) range

  let countsByYear = range
                     |> List.map (fun year -> CrimesThisYear crimes year)
                     |> List.map2 (fun year count -> (year, count)) range
  //
  // debugging: see Visual Studio's Output window (may need to scroll up?)
  //
  //printfn "Arrests: %A" arrests
  //printfn "Arrests by Year: %A" arrestsByYear
  //
  // plot:
  //
  
  let myChart =
    Chart.Combine([
                    Chart.Line(countsByYear, Name="Total # of Crimes")
                    Chart.Line(arrestsByYear, Name="# of Arrests")
                  ]) 
    
  let myChart2 = 
    myChart.WithTitle(filename).WithLegend();

  let myChartControl = 
    new ChartControl(myChart2, Dock=DockStyle.Fill)
  //
  // return back the chart for display:
  //
  myChartControl


//
// Given a list of crime tuples, returns a count of how many 
// crimes were reported for the given year and ICUR code:
//
let private CrimesByIUCRThisYear crimes years code= 
  let numCrimes = crimes 
                  |>List.filter (fun (_, iucr, _, _, _, year) -> year = years && iucr = code)
                  |>List.length
  numCrimes


//
// Arrests:
//
// Given a CSV file of crime data, analyzes # of crimes by IUCR, 
// returning a chart that can be displayed in a Windows desktop
// app:
//
let CrimesByIUCR(filename, code) = 
  //
  // debugging:  print filename, which appears in Visual Studio's Output window
  //
  //printfn "Calling CrimesByIUCR: %A" filename
  //
  let crimes = ParseCrimeData filename
  //
  let (_, _, _, _, _, minYear) = List.minBy (fun (_, _, _, _, _, year) -> year) crimes
  let (_, _, _, _, _, maxYear) = List.maxBy (fun (_, _, _, _, _, year) -> year) crimes
  
  let range = [minYear .. maxYear]
  let crimesByIucrs = range
                      |> List.map (fun year -> CrimesByIUCRThisYear crimes year code)
                      |> List.map2 (fun year count -> (year, count)) range

  let countsByYear = range
                     |> List.map (fun year -> CrimesThisYear crimes year)
                     |> List.map2 (fun year count -> (year, count)) range
  //
  // debugging: see Visual Studio's Output window (may need to scroll up?)
  //
  //printfn "Crime Count by IUCR: %A" iucrCount
  //printfn "Crimes by IUCR by Year: %A" crimesByIucrs

  //Get primary and secondary description for IUCR code
  let (iucr,primary,secondary) = ParseIUCR "IUCR-codes.csv"
                                 |> List.find(fun (iucr,_,_) -> iucr = code)
  let primaryDes = primary
  let secondaryDes = secondary
  let combineText = primary + ": " + secondary

  //
  // plot:
  //
  let myChart =
    Chart.Combine([
                    Chart.Line(countsByYear, Name="Total # of Crimes")
                    Chart.Line(crimesByIucrs, Name=combineText)
                  ]) 
    
  let myChart2 = 
    myChart.WithTitle(filename).WithLegend();

  let myChartControl = 
    new ChartControl(myChart2, Dock=DockStyle.Fill)
  //
  // return back the chart for display:
  //
  myChartControl


//
// Returns the code number for a given neighborhood 
// crimes were reported for the given year and area code:
//
let private neighborhoodCode neighborhood= 
  let areas = ParseArea "Areas.csv"
  let exists = List.exists(fun (code,name) -> name = neighborhood) areas
  if exists = false 
  then 0
  else
  let (areaCode, neighborhoodName) = List.find (fun (code,name) -> name = neighborhood) areas
  areaCode


//
// Given a list of crime tuples, returns a count of how many 
// crimes were reported for the given year and area code:
//
let private CrimesByAreaThisYear crimes years code= 
  let numCrimes = crimes
                  |> List.filter (fun (_, _, _, _, area, year) -> year = years && area = code)
                  |> List.length
  numCrimes

// Given a CSV file of crime data, analyzes # of crimes by area, 
//  for a year returning a chart that can be displayed in a Windows
//desktop app:
//
let CrimesByArea(filename, neighborhood) = 
  //get area code number
  let neighborhoodCode = neighborhoodCode neighborhood
  //
  // debugging:  print filename, which appears in Visual Studio's Output window
  //
  //printfn "Calling CrimesByArea: %A" filename
  //
  let crimes = ParseCrimeData filename
  //
  let (_, _, _, _, _, minYear) = List.minBy (fun (_, _, _, _, _, year) -> year) crimes
  let (_, _, _, _, _, maxYear) = List.maxBy (fun (_, _, _, _, _, year) -> year) crimes
  //
  let range = [minYear .. maxYear]
  let crimesByArea = range
                    |> List.map (fun year -> if neighborhoodCode = 0 then 0 else CrimesByAreaThisYear crimes year neighborhoodCode)
                    |> List.map2 (fun year count -> (year, count)) range

  let countsByYear = range
                    |> List.map (fun year -> CrimesThisYear crimes year)
                    |> List.map2 (fun year count -> (year, count)) range
  //
  // debugging: see Visual Studio's Output window (may need to scroll up?)
  //
  //printfn "Crimes by Area: %A" areaCount
  //printfn "Crimes by Area by Year: %A" crimesByArea
  //
  // plot:
  //
  let myChart =
    Chart.Combine([
                    Chart.Line(countsByYear, Name="Total # of Crimes")
                    Chart.Line(crimesByArea, Name=if neighborhoodCode = 0 then "Unknown Area" else neighborhood)
                  ]) 
    
  let myChart2 = 
    myChart.WithTitle(filename).WithLegend();

  let myChartControl = 
    new ChartControl(myChart2, Dock=DockStyle.Fill)
  //
  // return back the chart for display:
  //
  myChartControl


//
// Given a list of crime tuples, returns a count of how many 
// crimes were reported for the given year:
//
let private TotalCrimesInArea crimes code =
  let numCrimes = crimes 
                  |> List.filter (fun (_, _, _, _, area, _) -> area = code) 
                  |> List.length
  numCrimes


// Given a CSV file of crime data, analyzes # of crimes by area, 
// returning a chart that can be displayed in a Windows desktop
// app:
//
let TotalCrimesByArea(filename) = 
  // debugging:  print filename, which appears in Visual Studio's Output window
  //
  //printfn "Calling TotalCrimesByArea: %A" filename
  //
  let areas = ParseArea "Areas.csv"
  let crimes = ParseCrimeData filename
  //
  let (minArea,_) = List.minBy (fun (min,_) -> min) areas
  let (maxArea,_) = List.maxBy (fun (max,_) -> max) areas
  //
  let range = [minArea .. maxArea]
  let countsByArea = range
                     |> List.map (fun code -> TotalCrimesInArea crimes code)
                     |> List.map2 (fun area count -> (area, count)) range

  //
  // debugging: see Visual Studio's Output window (may need to scroll up?)
  //
  // printfn "Total Crimes: %A" counts
  //printfn "Total Crimes by Area: %A" countsByArea
  //
  // plot:
  //
  let myChart =
    Chart.Line(countsByArea, Name="Total Crimes By Chicago Area")
                    
  let myChart2 = 
    myChart.WithTitle(filename).WithLegend();

  let myChartControl = 
    new ChartControl(myChart2, Dock=DockStyle.Fill)
  //
  // return back the chart for display:
  //
  myChartControl


