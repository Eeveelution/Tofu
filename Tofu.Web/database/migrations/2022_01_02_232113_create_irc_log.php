<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;

class CreateIrcLog extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('irc_log', function (Blueprint $table) {
            $table->id();
            $table->string("channel")->nullable(false);
            $table->string("sender_name")->nullable(false);
            $table->bigInteger("sender_id")->nullable(false);
            $table->string("message")->nullable(false);
            $table->dateTime("time")->default(DB::raw('CURRENT_TIMESTAMP'))->nullable(false);
        });
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::dropIfExists('irc_log');
    }
}
