open System

module Envelope =

  type t = float * float * float * float

  let ranges_intersect a b a' b' = a' <= b && a <= b'

  let intersects (x0, x1, y0, y1) (x0', x1', y0', y1') =
    (* For two envelopes to intersect, both of their ranges do. *)
    ranges_intersect x0 x1 x0' x1' && ranges_intersect y0 y1 y0' y1'

  let add (x0, x1, y0, y1) (x0', x1', y0', y1') =
    min x0 x0', max x1 x1', min y0 y0', max y1 y1'

  let rec add_many = function
    | e :: [] -> e
    | e :: es -> add e (add_many es)
    | [] -> raise (ArgumentException "can't zero envelopes")

  let area (x0, x1, y0, y1) =
    (x1 - x0) * (y1 - y0)

  let within (x0, x1, y0, y1) (x0', x1', y0', y1') =
    x0 <= x0' && x1 >= x1' && y0 <= y0' && y1 >= y1'

  let empty = 0., 0., 0., 0.

module rtree =

  type 'a t =
      Node of (Envelope.t * 'a t) list
    | Leaf of (Envelope.t * 'a) list
    | Empty

  let max_node_load = 8

  let empty = Empty
  let empty_node = (Envelope.empty, Empty)

  let enlargement_needed e e' =
    Envelope.area (Envelope.add e e') - Envelope.area e

  let rec partition_by_min_enlargement e = function
    | (e', _) as n :: [] ->
        n, [], enlargement_needed e e'
    | (e', _) as n :: ns ->
        let enlargement = enlargement_needed e e' 
        let min, maxs, enlargement' = partition_by_min_enlargement e ns 
        if enlargement < enlargement' then
          n, min :: maxs, enlargement
        else
          min, n :: maxs, enlargement'
    | [] ->
        raise (ArgumentException "cannot partition an empty node")

  let pairs_of_list xs =  (* (cross product) *)
    List.concat (List.map (fun x -> List.map (fun y -> (x, y)) xs) xs)

  (* This is Guttman's quadradic splitting algorithm. *)
  let split_pick_seeds ns =
    let pairs = pairs_of_list ns 
    let cost (e0, _) (e1, _) =
      (Envelope.area (Envelope.add e0 e1)) -
      (Envelope.area e0) - (Envelope.area e1) 
    let rec max_cost = function
      | (n, n') :: [] -> cost n n', (n, n')
      | (n, n') as pair :: ns ->
          let max_cost', pair' = max_cost ns 
          let cost = cost n n' 
          if cost > max_cost' then
            cost, pair
          else
            max_cost', pair'
      | [] -> raise (ArgumentException "can't compute split on empty list") 
    let (_, groups) = max_cost pairs in groups

  let split_pick_next e0 e1 ns =
    let diff (e, _) =
      abs ((enlargement_needed e0 e) - (enlargement_needed e1 e)) 
    let rec max_difference = function
      | n :: [] -> diff n, n
      | n :: ns ->
          let diff', n' = max_difference ns 
          let diff = diff n 
          if diff > diff' then
            diff, n
          else
            diff', n'
      | [] -> raise (ArgumentException "can't compute max diff on empty list") 
    let (_, n) = max_difference ns in n

  let split_nodes ns =
    let rec partition xs xs_envelope ys ys_envelope = function
      | [] -> (xs, xs_envelope), (ys, ys_envelope)
      | rest -> 
          let (e, _) as n = split_pick_next xs_envelope ys_envelope rest 
          let rest' = List.filter ((<>) n) rest 
          let enlargement_x = enlargement_needed e xs_envelope 
          let enlargement_y = enlargement_needed e ys_envelope 
          if enlargement_x < enlargement_y then
            partition (n :: xs) (Envelope.add xs_envelope e) ys ys_envelope rest'
          else
            partition xs xs_envelope (n :: ys) (Envelope.add ys_envelope e) rest'
    let (((e0, _) as n0), ((e1, _) as n1)) = split_pick_seeds ns 
    partition [n0] e0 [n1] e1 (List.filter (fun n -> n <> n0 && n <> n1) ns)

  let envelope_of_nodes ns = Envelope.add_many (List.map (fun (e, _) -> e) ns)

  let rec insert' elem e = function
    | Node ns -> 
        let (_, min), maxs, _ = partition_by_min_enlargement e ns 
        match insert' elem e min with
          | min', (_, Empty) ->
              let ns' = min' :: maxs 
              let e' = envelope_of_nodes ns' 
              (e', Node ns'), empty_node
          | min', min'' when (List.length maxs + 2) < max_node_load ->
              let ns' = min' :: min'' :: maxs 
              let e' = envelope_of_nodes ns' 
              (e', Node ns'), empty_node
          | min', min'' ->
              let (a, envelope_a), (b, envelope_b) =
                split_nodes (min' :: min'' :: maxs) 
              (envelope_a, Node a), (envelope_b, Node b)
    | Leaf es ->
        let es' = (e, elem) :: es 
        if List.length es' > max_node_load then
          let (a, envelope_a), (b, envelope_b) = split_nodes es' 
          (envelope_a, Leaf a), (envelope_b, Leaf b)
        else
          (envelope_of_nodes es', Leaf es'), empty_node
    | Empty ->
        (e, Leaf [e, elem]), empty_node

  let insert t elem e =
    match insert' elem e t with
      | (_, a), (_, Empty) -> a
      | a, b -> Node [a; b]  (* root split *)

  let filter_intersecting e =
    List.filter (fun (e', _) -> Envelope.intersects e e')

  let rec find t e =
    match t with
      | Node ns ->
          let intersecting = filter_intersecting e ns 
          let found = List.map (fun (_, n) -> find n e) intersecting 
          List.concat found
      | Leaf es -> List.map snd (filter_intersecting e es)
      | Empty -> []

  let rec size = function
    | Node ns ->
        let sub_sizes = List.map (fun (_, n) -> size n) ns 
        List.fold (+) 0 sub_sizes
    | Leaf es ->
        List.length es
    | Empty ->
        0

