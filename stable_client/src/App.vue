<template>
  <div id="app">
    <b-navbar class="container" toggleable="lg" type="dark" variant="dark">
      <b-navbar-nav>
        <b-nav-item to="/home">Home</b-nav-item>
      </b-navbar-nav>
    </b-navbar>
    <router-view class="container" v-bind:rights="rights" v-bind:states="states" v-on:login="login">
    </router-view>
    <b-toast id="error_toast" variant="danger" solid toaster="b-toaster-top-center">
      <template v-slot:toast-title>
        <div class="d-flex flex-grow-1 align-items-baseline">
          <b-img blank blank-color="#ff5555" class="mr-2 rounded" width="12" height="12" ></b-img>
          <strong class="mr-auto">{{ toast.title }}</strong>
        </div>
      </template>
      {{ toast.body }}
    </b-toast>
  </div>
</template>

<script>
  import axios from 'axios';

  export default {
    name: 'App',
    components: {
    },
    data : function() {
      return {
        rights : {
          isConnected : false,
          isSecretary : false,
          isGroom : false,
          isBoss : false,
          isAdmin : false
        },
        baseUrl : 'http://localhost:5000/',
        states : {
          login : {
            running : false,
            error : false
          }
        },
        toast : {
          title : '',
          body  : ''
        }
      }
    },
    methods : {
      login : function (loginData) {
        let that = this;
        that.states.login.running = true
        axios.post(this.baseUrl + 'login', loginData, { withCredentials: true })
        .then()
        .catch(error => {
          that.toast.title = 'Error during login';
          that.toast.body = error.response ? error.response : error.message;
          that.$bvToast.show('error_toast');
          that.states.login.running = false;
        })

      },
      getRights : function () {
        let that = this;
        axios.get(this.baseUrl + 'rights', { withCredentials: true })
        .then(function (response) {
          that.rights = response.data;
        })
      }
    }
  }
</script>

<style lang="scss">
  @import url('https://fonts.googleapis.com/css?family=Roboto');
  body {
    background-color: #343a40 !important;
    font-family: 'roboto', sans-serif;
  }
  .nav-link {
    &.router-link-active {
      border-bottom: 1px solid;
    }
  }
</style>
