//http://www.urbit.org/2013/11/18/urbit-is-easy-ch2.html

namespace Nock

open System.Threading.Tasks

module Nock =

    type noun = 
        | Atom of int
        | Cell of noun * noun

    let AsyncCell (a : (_ -> noun), b : (_ -> noun)) : noun =
        let left = Task.Factory.StartNew(a);
        let right = Task.Factory.StartNew(b);
        Cell(left.Result, right.Result)

    let wut noun =
        match noun with
        | Cell(a, b) -> Atom(0)                                                                 // ?[a b]           0
        | Atom(a) -> Atom(1)                                                                    // ?a               1

    let lus noun =
        match noun with
        | Atom(a) -> Atom(1 + a)                                                                // +a               1 + a
        | Cell(_, _) -> raise (System.ArgumentException("Cannot lus a Cell"))

    let tis noun =
        match noun with
        | Cell(a, b) -> Atom(if a = b then 0 else 1)                                            // =[a a]           0
                                                                                                // =[a b]           1
        | Atom(_) -> raise (System.ArgumentException("Cannot tis an Atom"))

    let rec fas noun =
        match noun with
        | Cell (Atom(1), a) -> a                                                                // /[1 a]           a
        | Cell (Atom(2), Cell(a, b)) -> a                                                       // /[2 a b]         a
        | Cell (Atom(3), Cell(a, b)) -> b                                                       // /[3 a b]         b
        | Cell (Atom(a), b) -> 
            if a % 2 = 0 then
                fas(Cell(Atom(2), fas(Cell(Atom(a / 2), b))))                                   // /[(a + a) b]     /[2 /[a b]]
            else
                fas(Cell(Atom(3), fas(Cell(Atom((a - 1) / 2), b))))                             // /[(a + a + 1) b] /[3 /[a b]]
        | _ -> raise (System.ArgumentException("Invalid fas"))

    let rec tar noun : noun = 
        match noun with
        | Cell (a, Cell(Cell(b, c), d)) ->
            //Cell(tar(Cell(a, Cell(b, c))), tar(Cell(a, d)))                                   // *[a [b c] d]     [*[a b c] *[a d]]
            AsyncCell((fun _ -> tar(Cell(a, Cell(b, c)))), (fun _ -> tar(Cell(a, d))))
        | Cell (a, Cell(Atom(0), b)) -> fas(Cell(b, a))                                         // *[a 0 b]         /[b a]
        | Cell (a, Cell(Atom(1), b)) -> b                                                       // *[a 1 b]         b
        | Cell (a, Cell(Atom(2), Cell(b, c))) -> 
            //tar(Cell(tar(Cell(a, b)), tar(Cell(a, c))))                                       // *[a 2 b c]       *[*[a b] *[a c]]
            tar(AsyncCell((fun _ -> tar(Cell(a, b))), (fun _ -> tar(Cell(a, c)))));
        | Cell (a, Cell(Atom(3), b)) -> wut(tar(Cell(a, b)))                                    // *[a 3 b]         ?*[a b]
        | Cell (a, Cell(Atom(4), b)) -> lus(tar(Cell(a, b)))                                    // *[a 4 b]         +*[a b]
        | Cell (a, Cell(Atom(5), b)) -> tis(tar(Cell(a, b)))                                    // *[a 5 b]         =*[a b]
        | Cell (a, Cell(Atom(6), Cell(b, Cell(c, d)))) -> tar(Cell(a, Cell(Atom(2), Cell(Cell(Atom(0), Atom(1)), Cell(Atom(2), Cell(Cell(Atom(1), Cell(c, d)), Cell(Cell(Atom(1), Atom(0)), Cell(Atom(2), Cell(Cell(Atom(1), Cell(Atom(2), Atom(3))), Cell(Cell(Atom(1), Atom(0)), Cell(Atom(4), Cell(Atom(4), b))))))))))))     // *[a 6 b c d]     *[a 2 [0 1] 2 [1 c d] [1 0] 2 [1 2 3] [1 0] 4 4 b]
        | Cell (a, Cell(Atom(7), Cell(b, c))) -> tar(Cell(a, Cell(Atom(2), Cell(b, Cell(Atom(1), c)))))                                                            // *[a 7 b c]       *[a 2 b 1 c]
        | Cell (a, Cell(Atom(8), Cell(b, c))) -> tar(Cell(a, Cell(Atom(7), Cell(Cell(Cell(Atom(7), Cell(Cell(Atom(0), Atom(1)), b)), Cell(Atom(0), Atom(1))), c))))       // *[a 8 b c]       *[a 7 [[7 [0 1] b] 0 1] c]                                                          //<===========
        | Cell (a, Cell(Atom(9), Cell(b, c))) -> tar(Cell(a, Cell(Atom(7), Cell(c, Cell(Atom(2), Cell(Cell(Atom(0), Atom(1)), Cell(Atom(0), b)))))))               // *[a 7 c 2 [0 1] 0 b]
        | Cell (a, Cell(Atom(10), Cell(Cell(b, c), d))) -> tar(Cell(a, Cell(Atom(8), Cell(c, Cell(Atom(7), Cell(Cell(Atom(0), Atom(3)), d))))))                    // *[a 10 [b c] d]  *[a 8 c 7 [0 3] d]
        | Cell (a, Cell(Atom(10), Cell(b, c))) -> tar(Cell(a, c))                                                                                                  // *[a 10 b c]      *[a c]

        | Atom a -> raise (System.ArgumentException("Cannot nock an Atom"))                     // *a               *a
        | _ -> raise (System.ArgumentException("Invalid tar"))