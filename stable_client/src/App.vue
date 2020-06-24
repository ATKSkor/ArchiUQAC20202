<template>
  <div id="app">
    <b-navbar class="container" toggleable="lg" type="dark" variant="dark">
      <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>
      <b-collapse id="nav-collapse" is-nav>
        <b-navbar-nav>
          <b-nav-item to="/home">Home</b-nav-item>
          <b-nav-item v-if="rights.isAdmin || rights.isGroom || rights.isSecretary" to="/event">Event</b-nav-item>
          <b-nav-item v-if="rights.isAdmin || rights.isGroom || rights.isSecretary" to="/member">Member</b-nav-item>
          <b-nav-item v-if="rights.isAdmin || rights.isGroom" to="/horse">Horse</b-nav-item>
          <b-nav-item v-if="rights.isAdmin || rights.isGroom" to="/equipment">Equipment</b-nav-item>
          <b-nav-item v-if="rights.isAdmin" to="/admin">Admin</b-nav-item>
        </b-navbar-nav>
      </b-collapse>
      <b-navbar-nav class="ml-auto">
        <b-nav-item v-if="!rights.isConnected" disabled><em>Not logged in</em></b-nav-item>
        <b-nav-item v-on:click="logout" v-else>Logout</b-nav-item>
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
    mounted: function() {
      this.getRights();
    },
    data : function() {
      return {
        rights : {
          isConnected : false,
          isSecretary : false,
          isGroom : false,
          isAdmin : false
        },
        baseUrl : 'https://localhost:5001',
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
        axios.post(this.baseUrl + '/auth/login', loginData, { withCredentials: true })
                .then(() => {
                  that.getRights();
                  that.states.login.running = false;
                  that.states.login.error = false;
                })
                .catch(error => {
                  that.handleError("Error during login", error);
                  that.states.login.running = false;
                  that.states.login.error = true;
                })
      },
      getRights : function () {
        let that = this;
        axios.get(this.baseUrl + '/auth/rights', { withCredentials: true })
                .then(function (response) {
                  that.rights = response.data;
                }).catch(error => {
          that.handleError("Error while retrieving user rights", error)
          that.states.login.running = false;
          that.states.login.error = true;
          that.rights.isConnected = false;
        })
      },
      logout: function() {
        let that = this;
        axios.post(this.baseUrl + '/auth/logout', null, { withCredentials: true })
                .then (() => {
                  that.rights = {
                    isConnected : false,
                    isSecretary : false,
                    isGroom : false,
                    isAdmin : false
                  };
                  that.$router.push('/home');
                }).catch(error => {
          that.handleError("Error while logging out", error)
          that.rights.isConnected = false;
          that.getRights();
        })
      },
      handleError : function (title, error) {
        console.error(error);
        this.toast.title = title;
        if (error.response !== undefined && error.response.status !== undefined) {
          this.toast.body = "(http " + error.response.status + ") ";
        } else {
          this.toast.body = "(Status unknown) ";
        }
        if (error.response !== undefined && error.response.data !== undefined) {
          this.toast.body += error.response.data;
        } else if (error.message !== undefined) {
          this.toast.body += error.message;
        } else {
          this.toast += 'Unspecified error';
        }
        this.$bvToast.show('error_toast');
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
