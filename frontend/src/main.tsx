import React from 'react'
import ReactDOM from 'react-dom/client'
import './index.css'
import { RouterProvider, createBrowserRouter } from 'react-router-dom'
import { Home, Error, Dictionary, Subscription } from "./pages/_index.tsx";
import { FAQ, Footer, Menu, SubscriptionBanner } from './components/_index.tsx';

const router = createBrowserRouter([
  {
    path: "/",
    element: <Menu />,
    errorElement: <Error />,
    children: [
      {
        element: <Footer />
      }
    ]
  },
  {
    path: "subscription",
    element: <Subscription />,
  },
]);

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>
)
