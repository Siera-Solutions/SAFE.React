module Main

open Fable.Core.JsInterop
open Browser.Dom

// import global styles here
importSideEffects "./styles/global.scss"

let container = document.getElementById("feliz-app")

let root = React.createRoot container

root.render (Components.Router())