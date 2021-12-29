<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class UsersTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create("users", function(Blueprint $table) {
			$table->bigInteger("id")->autoIncrement()->nullable(false);
        	$table->timestamp("creation_time")->nullable(false)->default(DB::raw('CURRENT_TIMESTAMP'));
        	$table->timestamp("last_activity")->nullable(false)->default(DB::raw('CURRENT_TIMESTAMP'));
        	$table->string("username")->nullable(false)->unique()->index();
        	$table->string("password")->nullable(false);
        	$table->bigInteger("privileges", false, true)->nullable(false);
        	$table->string("email_adress")->unique()->index();
        	$table->timestamp("silenced_until");
        	$table->boolean("banned")->nullable(false)->default(false);
        	$table->string("location")->nullable(false)->default("osu!");
        	$table->string("user_page")->nullable(false)->default("");
		});
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
		Schema::dropIfExists('users');
    }
}
