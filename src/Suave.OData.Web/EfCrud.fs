namespace Suave.OData.Web
open System.Data.Entity
open Suave.OData.Core
open Suave.OData.EF
open System.Data.Entity.Migrations

[<AutoOpen>]
module EfCrud =

  let addEntity (db : DbContext) add entity = async {
    try
      add entity |> ignore
      let! _ = db.SaveChangesAsync() |> Async.AwaitTask
      return Some(entity)
    with
    | ex ->
      printfn "%A" ex
      return None
  }

  let findEntityById find (id : int) = async {
    try
      let! entity = find id |> Async.AwaitTask
      if isNull entity then
        return None
      else
        return Some(entity)
    with
    | ex ->
      printfn "%A" ex
      return None
  }

  let deleteEntityById (db : DbContext) find remove (id : int) = async {
      try
        let! entity = find id |> Async.AwaitTask
        if isNull entity then
          return None
        else
          remove entity |> ignore
          let! _ = db.SaveChangesAsync() |> Async.AwaitTask
          return Some(entity)
      with
      | ex ->
        printfn "%A" ex
        return None
    }

  let updateEntity(db : DbContext) update id entity = async {
    try
      update id entity
      let! _ = db.SaveChangesAsync() |> Async.AwaitTask
      return Some entity
    with
    | ex ->
      printf "%A" ex
      return None
  }


  let resource<'a when 'a : not struct and
        'a : equality and
        'a : null and 'a :> Entity> db name (dbSet : DbSet<'a>) =

    let update id (entity : 'a) =
      entity.ID <- id
      dbSet.AddOrUpdate entity
    {
      Name = name
      Entities = dbSet
      Add = addEntity db dbSet.Add
      UpdateById = updateEntity db update
      FindById = findEntityById dbSet.FindAsync
      DeleteById = deleteEntityById db dbSet.FindAsync dbSet.Remove
    }