import { BrowserRouter, Route, Routes } from "react-router-dom";
import Nav from "./components/Nav";
import Recipes from "./components/Recipes";

function App() {
  return (
    <BrowserRouter>
       <Nav />
      <Routes>
     <Route path="/recipes" element={<Recipes />}/>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
