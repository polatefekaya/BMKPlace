import { createContext } from "react";
import { RootContextModel } from "../models/rootModel";

export const RootContext = createContext();

function RootContextProvider({children}){
    return (
        <RootContext.Provider value={new RootContextModel()}>
            {children}
        </RootContext.Provider>
    );
}

export default RootContextProvider;