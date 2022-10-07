// Add packages when working in FSI using Alt+Enter
#if INTERACTIVE
#r "nuget: MongoDB.Driver"
#endif

//open Namespaces - similar to importing modules in JS. same as "using" in C#
open MongoDB.Driver
open MongoDB.Bson

// We use reflection to dump the values of any object.
let printProperties x =
    let t = x.GetType()
    let properties = t.GetProperties()
    printfn "-----------"
    printfn "%s" t.FullName
    properties |> Array.iter (fun prop ->
        if prop.CanRead then
            let value = prop.GetValue(x, null)
            printfn "%s: %O" prop.Name value
        else
            printfn "%s: ?" prop.Name)


let client = 
  MongoClient("")

type Deal = {
  Id: ObjectId
  Title: string
  PipedriveId: int64 option
}

let db = client.GetDatabase("braustin_crm")

let deals = db.GetCollection<BsonDocument>("deals")

let getAllDeals () =
  deals.Find(fun _ -> true).ToEnumerable()
  |> Seq.map( fun deal ->
    // deal.GetElement(2).ToString()
    let pipedriveId =
      let (foundit,x) = deal.TryGetValue("pipedriveId")
      if foundit then
        if x = BsonNull.Value then
          None
        else
          Some (System.Int64.Parse x.AsString)
      else
        None
    let dealName = deal["title"].AsString
    let objectId = deal["_id"].AsObjectId
    {
      Id = objectId
      Title = dealName
      PipedriveId = pipedriveId
    }
  )
  
for deal in getAllDeals () do
  printfn "%A" deal


