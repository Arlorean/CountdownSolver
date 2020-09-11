open System

module CountdownSolver =
    type Numbers = int list

    type Operation = Add | Sub | Mul | Div

    type Expr = {
        Op : Operation
        A : int
        B : int
    }

    let expr op a b =
        { Op=op; A=a; B=b; }

    let eval expr =
        match expr.Op with
        | Add -> expr.A + expr.B
        | Sub -> expr.A - expr.B
        | Mul -> expr.A * expr.B
        | Div -> expr.A / expr.B

    // Pairwise combinations of indices for an array of length 'n'
    // e.g. for n=6 -> (0,1),(0,2),(0,3),(0,4),(0,5),(1,2),(1,3),(1,4),(1,5),(2,3),(2,4),(2,5),(3,4),(3,5),(4,5)
    let permuteIndexPairs n =
        seq {
            for i = 0 to n-2 do
                for j = i+1 to n-1 do
                    yield (i,j)
        }

    let removeIndexPair (numbers:Numbers) (i,j) : Numbers =
        numbers |> List.indexed |> List.filter (fun (i',v) -> i' <> i && i' <> j) |> List.map snd

    let getValuePair (numbers:Numbers) (i,j) =
        numbers.[i], numbers.[j]
        
    let enumExpressions (a,b) =
        seq {
            yield expr Add a b 
            if a <> b then 
                yield expr Sub (max a b) (min a b) 
            if a <> 1 && b <> 1 then
                yield expr Mul a b 
            if a <> 1 && b <> 1 && a <> b && (a%b=0 || b%a=0) then
                yield expr Div (max a b) (min a b) 
        } 

    type Solution = {
        Target : int
        Numbers : Numbers
        Expressions : Expr list
    }

    let rec enumSolutions solution : Solution seq =
        let numbers = solution.Numbers
        let indexPairs = permuteIndexPairs (List.length numbers)
        let valuePairs = indexPairs |> Seq.map (getValuePair numbers)
        let newNumbers = indexPairs |> Seq.map (removeIndexPair numbers)
        
        seq {
            yield solution
            yield!
                Seq.zip valuePairs newNumbers
                |> Seq.collect (
                    fun ((a,b),n) -> enumExpressions (a,b) |> Seq.map (fun e -> e,n)
                )
                |> Seq.collect (
                    fun (expr:Expr,numbers:Numbers) -> 
                        enumSolutions {
                            Target = solution.Target
                            Numbers=(eval expr)::numbers; 
                            Expressions=expr::solution.Expressions
                        }
                )
        }

    let distanceToTarget solution =
        solution.Numbers |> Seq.map (fun i -> abs (i - solution.Target)) |> Seq.min

    let isExactSolution solution =
        distanceToTarget solution = 0

    // Closest to the target number, with the smallest number of expressions used
    let findBestSolution problem = 
        enumSolutions problem
        |> Seq.sortBy (
            fun solution -> distanceToTarget solution, solution.Expressions.Length
        )
        |> Seq.head

    let findExactSolution problem =
        enumSolutions problem
        |> Seq.tryFind isExactSolution

    let printOp op =
        match op with
        | Add -> "+"
        | Sub -> "-"
        | Mul -> "*"
        | Div -> "/"

    let printExpr expr =
        printfn "%i %s %i = %i" expr.A (printOp expr.Op) expr.B (eval expr)

    let printSolution solution =
        printfn "Target=%i, Numbers=%A" solution.Target solution.Numbers
        solution.Expressions |> List.rev |> List.iter printExpr


open CountdownSolver

let findBestSolutionTimed problem = 
    let timer = System.Diagnostics.Stopwatch.StartNew()
    let result = findBestSolution problem
    printfn "Time to find best solution: %A" timer.Elapsed
    result

let findExactSolutionTimed problem = 
    let timer = System.Diagnostics.Stopwatch.StartNew()
    let result = findExactSolution problem
    printfn "Time to try to find exact solution: %A" timer.Elapsed
    result

let runTest problem =
    enumSolutions problem |> Seq.filter isExactSolution |> Seq.length |> printfn "%i exact solutions available"
    findExactSolutionTimed problem |> ignore
    findBestSolutionTimed problem |> printSolution
    printfn "--------------------------------------------------"

/// Test from http://www.maths-resources.com/countdown/practise.html#numbers
/// 50 * 2 = 100
/// 4 - 1 = 3
/// 100 + 75 = 175
/// 175 + 3 = 178
let problem1 = {
    Target = 178
    Numbers = [50;75;2;4;1;4]
    Expressions = []
}

/// https://www.youtube.com/watch/?v=pfa3MHLLSWI
/// 100 + 6 = 106
/// 106 * 3 = 318
/// 318 * 75 = 23850
/// 23850 - 50 = 23800
/// 23800 / 25 = 952
let problem2 = {
    Target = 952
    Numbers = [25;50;75;100;3;6]
    Expressions = []
}

// https://forums.digitalspy.com/discussion/1397845/countdown-numbers-game-when-it-cant-be-done
let problem3 = {
    Target = 824
    Numbers = [3;7;6;2;1;7]
    Expressions = []
}

[<EntryPoint>]
let main argv =
    runTest problem1 
    runTest problem2 
    runTest problem3 

    0 // return an integer exit code
