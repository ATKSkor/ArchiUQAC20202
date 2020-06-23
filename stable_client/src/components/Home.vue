<template>
    <div class="pt-5">
        <h1 class="text-center">Welcome to stable</h1>
        <div v-if="!rights.isConnected" >
            <h2 class="text-center pt-5 pb-3">Please, login</h2>
            <div class="text-center">
                <b-row class="pb-2">
                    <b-col lg="3"></b-col>
                    <b-col cols="3" lg="2" class="">
                        <label for="username" class="m-0">Username</label>
                    </b-col>
                    <b-col cols="9" lg="4" class="">
                        <b-form-input id="username" size="sm" type="text" trim v-model="login.login"></b-form-input>
                    </b-col>
                    <b-col lg="3"></b-col>
                </b-row>
                <b-row>
                    <b-col lg="3"></b-col>
                    <b-col cols="3" lg="2" class="">
                        <label for="password" class="m-0">Password</label>
                    </b-col>
                    <b-col cols="9" lg="4" class="">
                        <b-form-input id="password" size="sm" type="password" v-model="login.pass"></b-form-input>
                    </b-col>
                    <b-col lg="3"></b-col>
                </b-row>
                <b-row>
                    <b-col lg="4"></b-col>
                    <b-col cols="12" lg="4" class="pt-2">
                        <b-btn
                                v-on:click="$emit('login', login)"
                                :disabled="states.login.running"
                                variant="secondary"
                                class="w-100"
                        >
                            <b-spinner small variant="light" label="loading" v-if="states.login.running"></b-spinner>
                            <span v-else>Login</span>
                        </b-btn>
                    </b-col>
                    <b-col lg="4"></b-col>
                </b-row>
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        name: "Home",
        props: {
            rights : Object,
            states : Object
        },
        data: function () {
            return {
                login : {
                    login : '',
                    pass : ''
                }
            }
        }
    }
</script>

<style lang="scss" scoped>
    * {
        color: rgba(255, 255, 255, 0.8);
    }
    .rounded-top-left {
        border-top-left-radius: .25rem !important;
    }
    .rounded-top-right {
        border-top-right-radius: .25rem !important;
    }
    .rounded-bottom-right {
        border-bottom-right-radius: .25rem !important;
    }
    .rounded-bottom-left {
        border-bottom-left-radius: .25rem !important;
    }
    .form-control,
    input:-webkit-autofill,
    input:-webkit-autofill:hover,
    input:-webkit-autofill:focus,
    input:-webkit-autofill:active
    {
        -webkit-text-fill-color:rgba(255, 255, 255, 0.8) !important;
        color: rgba(255, 255, 255, 0.8) !important;
        background-color: rgba(255, 255, 255, 0.2) !important;
    }
</style>