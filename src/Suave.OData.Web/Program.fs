namespace Suave.OData.Web

open Suave.OData.EF
open Suave
open Suave.Web
open System.Data.Entity
open Suave.OData.Core

module Main =
  [<EntryPoint>]
  let main argv =
    let db = new Db()
    let app = resource db "people" (db.People) |> OData.CRUD
    startWebServer defaultConfig app
    0
