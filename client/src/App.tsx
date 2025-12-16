import { BrowserRouter, Route, Routes } from "react-router-dom";
import Nav from "./components/Nav";
import Recipes from "./components/Recipes";
import RecipeDetail from "./components/RecipeDetail";

function App() {
  return (
    <BrowserRouter>
       <Nav />
      <Routes>
     <Route path="/recipes" element={<Recipes />}/>
        <Route path="/recipe/:id" element={<RecipeDetail />}/>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
