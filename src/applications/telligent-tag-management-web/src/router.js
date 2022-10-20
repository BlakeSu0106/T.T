import Vue from 'vue';
import Router from 'vue-router';


Vue.use(Router);

const router = new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
            {
              path: '',
              component: () => import('./views/Home'),
              children: [
                {
                  path: '',
                  component: () => import('./views/TagManagement'),
             
                },
              ],  
            },
            {
              path: '',
              component: () => import('./views/Home'),
              children: [
                {
                  path: '',
                  component: () => import('./views/TagManagement'),
             
                },
              ], 
            }
          ],
});






export default router;
