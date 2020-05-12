module fsharp_core.Data.CoreContext
open Microsoft.EntityFrameworkCore

type CoreContext(options: DbContextOptions<CoreContext>) =
  inherit DbContext(options)
